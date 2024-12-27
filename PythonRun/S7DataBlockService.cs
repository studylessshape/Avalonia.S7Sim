using PipeProtocol;
using S7Sim.Services;

namespace PythonRun
{
    internal class S7DataBlockService : IS7DataBlockService
    {
        private readonly string pipeName;

        public S7DataBlockService(string pipeName, string module)
        {
            this.pipeName = pipeName;
            Module = module;
        }

        public string Module { get; }

        public bool ReadBit(int dbNumber, int offset, byte bit)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), offset.ToString(), bit.ToString()]);
            return bool.Parse(response.Message);
        }

        public byte ReadByte(int dbNumber, int pos)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString()]);
            return byte.Parse(response.Message);
        }

        public double ReadDouble(int dbNumber, int pos)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString()]);
            return byte.Parse(response.Message);
        }

        public int ReadInt32(int dbNumber, int pos)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString()]);
            return int.Parse(response.Message);
        }

        public long ReadLong(int dbNumber, int pos)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString()]);
            return long.Parse(response.Message);
        }

        public float ReadReal(int dbNumber, int pos)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString()]);
            return float.Parse(response.Message);
        }

        public short ReadShort(int dbNumber, int pos)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString()]);
            return short.Parse(response.Message);
        }

        public string ReadString(int dbNumber, int offset)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), offset.ToString()]);
            return response.Message;
        }

        public uint ReadUInt32(int dbNumber, int pos)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString()]);
            return uint.Parse(response.Message);
        }

        public ulong ReadULong(int dbNumber, int pos)
        {
            var response = ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString()]);
            return ulong.Parse(response.Message);
        }

        public void WriteBit(int dbNumber, int offset, byte bit, bool flag)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), offset.ToString(), bit.ToString(), flag.ToString()]);
        }

        public void WriteByte(int dbNumber, int pos, byte value)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString(), value.ToString()]);
        }

        public void WriteDouble(int dbNumber, int pos, double value)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString(), value.ToString()]);
        }

        public void WriteInt32(int dbNumber, int pos, int value)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString(), value.ToString()]);
        }

        public void WriteLong(int dbNumber, int pos, long value)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString(), value.ToString()]);
        }

        public void WriteReal(int dbNumber, int pos, float value)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString(), value.ToString()]);
        }

        public void WriteShort(int dbNumber, int pos, short value)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString(), value.ToString()]);
        }

        public void WriteString(int dbNumber, int offset, int maxlen, string str)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), offset.ToString(), maxlen.ToString(), $"\"{str}\""]);
        }

        public void WriteUInt32(int dbNumber, int pos, uint value)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString(), value.ToString()]);
        }

        public void WriteULong(int dbNumber, int pos, ulong value)
        {
            ProtocolTools.SendCommand(pipeName, Module, nameof(ReadBit), [dbNumber.ToString(), pos.ToString(), value.ToString()]);
        }
    }
}
