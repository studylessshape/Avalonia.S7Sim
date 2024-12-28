using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipeProtocol
{
    public static class ProtocolTools
    {
        public static PipeResponse SendCommand(string pipeName, string module, string methodName, params string[] parameters)
        {
            using var clientStream = new NamedPipeClientStream(pipeName);
            clientStream.Connect(10);

            ProtocolTools.SendCommand(clientStream, new PipeCommand()
            {
                Module = module,
                Method = methodName,
                Parameters = parameters ?? Array.Empty<string>(),
            });

            var response = ProtocolTools.ReadResponse(clientStream);
            if (response.ErrCode != 0)
            {
                throw new Exception($"Execute command get code {response.ErrCode}, error message: {response.Message}");
            }

            return response;
        }

        public static byte[] Read(Stream pipeStream, int count)
        {
            var buffer = new byte[count];
            var readLength = pipeStream.Read(buffer, 0, count);
            if (readLength > 0)
            {
                return buffer;
            }
            else
            {
                return Array.Empty<byte>();
            }
        }

        public static async Task<byte[]> ReadAsync(Stream pipeStream, int count)
        {
            var buffer = new byte[count];
            var readLength = await pipeStream.ReadAsync(buffer, 0, count);
            if (readLength > 0)
            {
                return buffer;
            }
            else
            {
                return Array.Empty<byte>();
            }
        }

        public static void Send(Stream pipeStream, string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message ?? "");
            var head = new Head() { TotalIndex = 1, CurrentIndex = 1, ContentLength = messageBytes.Length };

            var sendBytes = head.GetBytes().Concat(messageBytes).ToArray();
            pipeStream.Write(sendBytes, 0, sendBytes.Length);
        }

        public static async Task SendAsync(Stream pipeStream, string message, CancellationToken? stopToken = null)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message ?? "");
            var head = new Head() { TotalIndex = 1, CurrentIndex = 1, ContentLength = messageBytes.Length };

            var sendBytes = head.GetBytes().Concat(messageBytes).ToArray();

            if (stopToken != null)
            {
                await pipeStream.WriteAsync(sendBytes, 0, sendBytes.Length, stopToken.Value);
            }
            else
            {
                await pipeStream.WriteAsync(sendBytes, 0, sendBytes.Length);
            }
        }

        public static void SendCommand(Stream pipeStream, PipeCommand pipeCommand)
        {
            Send(pipeStream, pipeCommand.ToCommand());
        }

        public static async Task SendCommandAsync(Stream pipeStream, PipeCommand pipeCommand)
        {
            await SendAsync(pipeStream, pipeCommand.ToCommand());
        }

        static Head ReadHead(Stream stream)
        {
            var buffer = new byte[6].AsSpan();
            var readLength = stream.Read(buffer);
            if (readLength < 6)
            {
                throw new IndexOutOfRangeException("Received too short head");
            }
            return Head.ToHead(buffer);
        }

        static async Task<Head> ReadHeadAsync(Stream stream, CancellationToken stopToken = default)
        {
            var buffer = new byte[6].AsMemory();
            var readLength = await stream.ReadAsync(buffer, stopToken);
            if (readLength < 6)
            {
                throw new IndexOutOfRangeException("Received too short head");
            }
            return Head.ToHead(buffer);
        }

        static string ReadContent(Stream stream, int length, CancellationToken stopToken)
        {
            StringBuilder builder = new StringBuilder();
            while (length > 0 && stopToken.IsCancellationRequested == false)
            {
                var buffer = new byte[1024].AsSpan();
                var readLength = stream.Read(buffer);
                if (readLength == 0)
                {
                    break;
                }

                builder.Append(Encoding.UTF8.GetString(buffer[..readLength]));
                length -= readLength;
            }

            return builder.ToString();
        }

        static async Task<string> ReadContentAsync(Stream stream, int length, CancellationToken stopToken)
        {
            StringBuilder builder = new StringBuilder();
            while (length > 0 && stopToken.IsCancellationRequested == false)
            {
                var buffer = new byte[1024].AsMemory();
                var readLength = await stream.ReadAsync(buffer, stopToken);
                if (readLength == 0)
                {
                    break;
                }

                builder.Append(Encoding.UTF8.GetString(buffer.Span[..readLength]));
                length -= readLength;
            }

            return builder.ToString();
        }

        public static string Read(Stream pipeStream, CancellationToken stopToken = default)
        {
            var head = ReadHead(pipeStream);

            return ReadContent(pipeStream, head.ContentLength, stopToken);
        }

        public static async Task<string> ReadAsync(Stream pipeStream, CancellationToken stopToken = default)
        {
            var head = await ReadHeadAsync(pipeStream, stopToken);

            return await ReadContentAsync(pipeStream, head.ContentLength, stopToken);
        }

        public static PipeCommand ReadCommand(Stream pipeStream, CancellationToken stopToken = default)
        {
            return PipeCommand.Parse(Read(pipeStream, stopToken));
        }

        public static async Task<PipeCommand> ReadCommandAsync(Stream pipeStream, CancellationToken stopToken = default)
        {
            return PipeCommand.Parse(await ReadAsync(pipeStream, stopToken));
        }

        public static void SendResponse(Stream pipeStream, int errCode, string msg)
        {
            var head = new Head()
            {
                TotalIndex = 1,
                CurrentIndex = 1,
                ContentLength = msg == null ? 0 : msg.Length
            };

            var errCodeBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(errCode));

            var sendBytes = head.GetBytes().Concat(errCodeBytes).Concat(Encoding.UTF8.GetBytes(msg ?? "")).ToArray();
            pipeStream.Write(sendBytes, 0, sendBytes.Length);
        }

        public static async Task SendResponseAsync(Stream pipeStream, PipeResponse response, CancellationToken stopToken = default)
        {
            var head = new Head()
            {
                TotalIndex = 1,
                CurrentIndex = 1,
                ContentLength = response.Message == null ? 0 : response.Message.Length
            };

            var errCodeBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(response.ErrCode));

            var sendBytes = head.GetBytes().Concat(errCodeBytes).Concat(Encoding.UTF8.GetBytes(response.Message ?? "")).ToArray();
            await pipeStream.WriteAsync(sendBytes, 0, sendBytes.Length, stopToken);
        }

        public static PipeResponse ReadResponse(Stream pipeStream, CancellationToken stopToken = default)
        {
            var head = ReadHead(pipeStream);

            var errCodeBytes = Read(pipeStream, 4);
            if (errCodeBytes.Length != 4)
            {
                throw new IndexOutOfRangeException("Received too short error code");
            }

            var errCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(errCodeBytes));
            var content = ReadContent(pipeStream, head.ContentLength, stopToken);

            return new PipeResponse
            {
                ErrCode = errCode,
                Message = content,
            };
        }

        public static async Task<PipeResponse> ReadResponseAsync(Stream pipeStream, CancellationToken stopToken = default)
        {
            var head = await ReadHeadAsync(pipeStream);

            var errCodeBytes = await ReadAsync(pipeStream, 4);
            if (errCodeBytes.Length != 4)
            {
                throw new IndexOutOfRangeException("Received too short error code");
            }

            var errCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(errCodeBytes));
            var content = await ReadContentAsync(pipeStream, head.ContentLength, stopToken);

            return new PipeResponse
            {
                ErrCode = errCode,
                Message = content,
            };
        }
    }
}
