using System;
using System.Linq;
using System.Net;

namespace PipeProtocol
{
    internal struct Head
    {
        public byte TotalIndex;
        public byte CurrentIndex;
        public int ContentLength;

        public byte[] GetBytes()
        {
            var lenBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(ContentLength));
            byte[] indexBytes = new byte[] { TotalIndex, CurrentIndex };
            return indexBytes.Concat(lenBytes).ToArray();
        }

        public static Head ToHead(byte[] bytes)
        {
            return ToHead(bytes.AsSpan());
        }

        public static Head ToHead(Span<byte> bytes)
        {
            return new Head
            {
                TotalIndex = bytes[0],
                CurrentIndex = bytes[1],
                ContentLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes.Slice(2, 4).ToArray(), 0))
            };
        }

        public static Head ToHead(Memory<byte> bytes)
        {
            return ToHead(bytes.Span);
        }
    }
}
