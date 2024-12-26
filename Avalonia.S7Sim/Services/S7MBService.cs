using FutureTech.Snap7;
using S7Sim.Services;
using S7Sim.Services.Exceptions;
using S7Sim.Services.Models;
using System.Linq;

namespace Avalonia.S7Sim.Services
{
    public class S7MBService : IS7MBService
    {
        private readonly IS7ServerService _serverService;

        public S7MBService(IS7ServerService s7ServerService)
        {
            _serverService = s7ServerService;
        }

        public byte[] GetBuffer()
        {
            var config = _serverService.RunningItems.FirstOrDefault(i => i.AreaKind == AreaKind.MB);
            if (config == null)
            {
                throw new MbNotExistException();
            }
            var buffer = config.Bytes;
            return buffer;
        }

        #region Byte
        public byte ReadByte(int pos)
        {
            var buffer = GetBuffer();
            var val = S7.GetByteAt(buffer, pos);
            return val;
        }


        public void WriteByte(int pos, byte value)
        {
            var buffer = GetBuffer();
            S7.SetByteAt(buffer, pos, value);
        }
        #endregion

        #region Short
        public short ReadShort(int pos)
        {
            var buffer = GetBuffer();

            var val = S7.GetIntAt(buffer, pos);
            return (short)val;
        }


        public void WriteShort(int pos, short value)
        {
            var buffer = GetBuffer();

            S7.SetIntAt(buffer, pos, value);
        }
        #endregion


        #region Bit
        public bool ReadBit(int offset, byte bit)
        {
            var buffer = GetBuffer();

            var x = S7.GetBitAt(buffer, offset, bit);
            return x;
        }


        public void WriteBit(int offset, byte bit, bool flag)
        {
            var buffer = GetBuffer();

            S7.SetBitAt(ref buffer, offset, bit, flag);
        }
        #endregion

        #region String
        public void WriteString(int offset, int maxlen, string str)
        {
            var buffer = GetBuffer();
            S7.SetStringAt(buffer, offset, maxlen, str);
        }

        public string ReadString(int offset)
        {
            var buffer = GetBuffer();
            var str = S7.GetStringAt(buffer, offset);
            return str;
        }
        #endregion

        #region Real
        public void WriteReal(int pos, float real)
        {
            var buffer = GetBuffer();
            S7.SetRealAt(buffer, pos, real);
        }

        public float ReadReal(int pos)
        {
            var buffer = GetBuffer();
            var real = S7.GetRealAt(buffer, pos);
            return real;
        }

        #endregion

        #region ulong
        public ulong ReadULong(int pos)
        {
            var buffer = GetBuffer();

            var val = S7.GetULIntAt(buffer, pos);
            return val;
        }


        public void WriteULong(int pos, ulong value)
        {
            var buffer = GetBuffer();

            S7.SetULintAt(buffer, pos, value);
        }
        #endregion

        #region uint32
        public uint ReadUInt32(int pos)
        {
            var buffer = GetBuffer();

            var val = S7.GetUDIntAt(buffer, pos);
            return val;
        }

        public void WriteUInt32(int pos, uint value)
        {
            var buffer = GetBuffer();

            S7.SetUDIntAt(buffer, pos, value);
        }
        #endregion
    }
}