using FutureTech.Snap7;
using System;

namespace S7Sim.Services
{
    public class ClientDataBlockService : IS7DataBlockService
    {
        private S7Client _client = new S7Client();
        private string? address = null;
        private int Rack = 0;
        private int Slot = 102;

        public bool ConnectTo(string address, int Rack = 0, int Slot = 102)
        {
            this.address = address;
            this.Rack = Rack;
            this.Slot = Slot;
            return _client.ConnectTo(this.address, this.Rack, this.Slot) == 0;
        }

        public bool EnsureConnect()
        {
            if (this.address == null)
            {
                return false;
            }

            if (_client.Connected())
            {
                return true;
            }

            return ConnectTo(address);
        }

        public bool ReadBit(int dbNumber, int offset, byte bit)
        {
            if (!EnsureConnect())
            {
                throw new Exception("Can't connect s7 server");
            }

            throw new NotImplementedException();
        }

        public byte ReadByte(int dbNumber, int pos)
        {
            throw new NotImplementedException();
        }

        public double ReadDouble(int dbNumber, int pos)
        {
            throw new NotImplementedException();
        }

        public int ReadInt32(int dbNumber, int pos)
        {
            throw new NotImplementedException();
        }

        public long ReadLong(int dbNumber, int pos)
        {
            throw new NotImplementedException();
        }

        public float ReadReal(int dbNumber, int pos)
        {
            throw new NotImplementedException();
        }

        public short ReadShort(int dbNumber, int pos)
        {
            throw new NotImplementedException();
        }

        public string ReadString(int dbNumber, int offset)
        {
            throw new NotImplementedException();
        }

        public uint ReadUInt32(int dbNumber, int pos)
        {
            throw new NotImplementedException();
        }

        public ulong ReadULong(int dbNumber, int pos)
        {
            throw new NotImplementedException();
        }

        public void WriteBit(int dbNumber, int offset, byte bit, bool flag)
        {
            throw new NotImplementedException();
        }

        public void WriteByte(int dbNumber, int pos, byte value)
        {
            throw new NotImplementedException();
        }

        public void WriteDouble(int dbNumber, int pos, double real)
        {
            throw new NotImplementedException();
        }

        public void WriteInt32(int dbNumber, int pos, int value)
        {
            throw new NotImplementedException();
        }

        public void WriteLong(int dbNumber, int pos, long value)
        {
            throw new NotImplementedException();
        }

        public void WriteReal(int dbNumber, int pos, float real)
        {
            throw new NotImplementedException();
        }

        public void WriteShort(int dbNumber, int pos, short value)
        {
            throw new NotImplementedException();
        }

        public void WriteString(int dbNumber, int offset, int maxlen, string str)
        {
            throw new NotImplementedException();
        }

        public void WriteUInt32(int dbNumber, int pos, uint value)
        {
            throw new NotImplementedException();
        }

        public void WriteULong(int dbNumber, int pos, ulong value)
        {
            throw new NotImplementedException();
        }
    }
}
