using S7Sim.Services;

namespace PythonRun
{
    public class S7MBService : IS7MBService
    {
        private readonly string pipeName;

        public S7MBService(string pipeName, string module)
        {
            this.pipeName = pipeName;
            Module = module;
        }

        public string Module { get; }

        #region Byte
        public byte ReadByte(int pos)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(ReadByte), [pos.ToString()]);
            return byte.Parse(response.Message);
        }


        public void WriteByte(int pos, byte value)
        {
            ProtocolExtra.SendCommand(pipeName, Module, nameof(WriteByte), [pos.ToString(), value.ToString()]);
        }
        #endregion

        #region Short
        public short ReadShort(int pos)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(ReadShort), [pos.ToString()]);
            return short.Parse(response.Message);
        }


        public void WriteShort(int pos, short value)
        {
            ProtocolExtra.SendCommand(pipeName, Module, nameof(WriteShort), [pos.ToString(), value.ToString()]);
        }
        #endregion


        #region Bit
        public bool ReadBit(int offset, byte bit)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(ReadBit), [offset.ToString(), bit.ToString()]);
            return bool.Parse(response.Message);
        }


        public void WriteBit(int offset, byte bit, bool flag)
        {
            ProtocolExtra.SendCommand(pipeName, Module, nameof(WriteBit), [offset.ToString(), bit.ToString(), flag.ToString()]);
        }
        #endregion

        #region String
        public void WriteString(int offset, int maxlen, string str)
        {
            ProtocolExtra.SendCommand(pipeName, Module, nameof(WriteString), [offset.ToString(), maxlen.ToString(), $"\"{str.Replace("\"", "\\\"")}\""]);
        }

        public string ReadString(int offset)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(ReadString), [offset.ToString()]);
            return response.Message;
        }
        #endregion

        #region Real
        public void WriteReal(int pos, float real)
        {
            ProtocolExtra.SendCommand(pipeName, Module, nameof(WriteReal), [pos.ToString(), real.ToString()]);
        }

        public float ReadReal(int pos)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(ReadReal), [pos.ToString()]);
            return float.Parse(response.Message);
        }
        #endregion

        #region ulong
        public ulong ReadULong(int pos)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(ReadULong), [pos.ToString()]);
            return ulong.Parse(response.Message);
        }


        public void WriteULong(int pos, ulong value)
        {
            ProtocolExtra.SendCommand(pipeName, Module, nameof(WriteULong), [pos.ToString(), value.ToString()]);
        }
        #endregion

        #region uint32
        public uint ReadUInt32(int pos)
        {
            var response = ProtocolExtra.SendCommand(pipeName, Module, nameof(ReadUInt32), [pos.ToString()]);
            return uint.Parse(response.Message);
        }

        public void WriteUInt32(int pos, uint value)
        {
            ProtocolExtra.SendCommand(pipeName, Module, nameof(WriteUInt32), [pos.ToString(), value.ToString()]);
        }
        #endregion
    }
}