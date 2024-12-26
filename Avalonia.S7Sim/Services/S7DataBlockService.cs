using FutureTech.Snap7;
using S7Sim.Services;
using S7Sim.Services.Exceptions;
using S7Sim.Services.Models;
using System.Linq;

namespace Avalonia.S7Sim.Services
{
    public class S7DataBlockService : IS7DataBlockService
    {
        private readonly IS7ServerService _serverService;

        public S7DataBlockService(IS7ServerService s7ServerService)
        {
            _serverService = s7ServerService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbNumber"></param>
        /// <returns></returns>
        /// <exception cref="DbNumberNotExistException"></exception>
        private RunningServerItem GetConfig(int dbNumber)
        {
            var config = _serverService.RunningItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            return config ?? throw new DbNumberNotExistException(dbNumber);
        }

        #region Byte
        public byte ReadByte(int dbNumber, int pos)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            var val = S7.GetByteAt(buffer, pos);
            return val;
        }


        public void WriteByte(int dbNumber, int pos, byte value)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            S7.SetByteAt(buffer, pos, value);
        }
        #endregion

        #region Short
        public short ReadShort(int dbNumber, int pos)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            var val = S7.GetIntAt(buffer, pos);
            return (short)val;
        }


        public void WriteShort(int dbNumber, int pos, short value)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            S7.SetIntAt(buffer, pos, value);
        }
        #endregion

        #region Int32
        public int ReadInt32(int dbNumber, int pos)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            var val = S7.GetDIntAt(buffer, pos);
            return val;
        }

        public void WriteInt32(int dbNumber, int pos, int value)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            S7.SetDIntAt(buffer, pos, value);
        }
        #endregion

        #region Long
        public long ReadLong(int dbNumber, int pos)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            var val = S7.GetLIntAt(buffer, pos);
            return val;
        }

        public void WriteLong(int dbNumber, int pos, long value)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;
            S7.SetLIntAt(buffer, pos, value);
        }
        #endregion

        #region Bit
        public bool ReadBit(int dbNumber, int offset, byte bit)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            var x = S7.GetBitAt(buffer, offset, bit);
            return x;
        }


        public void WriteBit(int dbNumber, int offset, byte bit, bool flag)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            S7.SetBitAt(ref buffer, offset, bit, flag);
        }
        #endregion

        #region String
        public void WriteString(int dbNumber, int offset, int maxlen, string str)
        {
            var config = GetConfig(dbNumber);
            var buffer = config.Bytes;
            S7.SetStringAt(buffer, offset, maxlen, str);
        }

        public string ReadString(int dbNumber, int offset)
        {
            var config = GetConfig(dbNumber);
            var buffer = config.Bytes;
            var str = S7.GetStringAt(buffer, offset);
            return str;
        }
        #endregion

        #region Real
        public void WriteReal(int dbNumber, int pos, float real)
        {
            var config = GetConfig(dbNumber);
            var buffer = config.Bytes;
            S7.SetRealAt(buffer, pos, real);
        }

        public float ReadReal(int dbNumber, int pos)
        {
            var config = GetConfig(dbNumber);
            var buffer = config.Bytes;
            var real = S7.GetRealAt(buffer, pos);
            return real;
        }

        #endregion

        #region Double
        public double ReadDouble(int dbNumber, int pos)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            return S7.GetLRealAt(buffer, pos);
        }

        public void WriteDouble(int dbNumber, int pos, double real)
        {
            var config = GetConfig(dbNumber);
            var buffer = config.Bytes;

            S7.SetLRealAt(buffer, pos, real);
        }

        #endregion

        #region ulong
        public ulong ReadULong(int dbNumber, int pos)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            var val = S7.GetULIntAt(buffer, pos);
            return val;
        }

        public void WriteULong(int dbNumber, int pos, ulong value)
        {
            var config = GetConfig(dbNumber);
            var buffer = config.Bytes;

            S7.SetULintAt(buffer, pos, value);
        }
        #endregion

        #region uint32
        public uint ReadUInt32(int dbNumber, int pos)
        {
            var config = GetConfig(dbNumber);

            var buffer = config.Bytes;

            var val = S7.GetUDIntAt(buffer, pos);
            return val;
        }

        public void WriteUInt32(int dbNumber, int pos, uint value)
        {
            var config = GetConfig(dbNumber);
            var buffer = config.Bytes;

            S7.SetUDIntAt(buffer, pos, value);
        }
        #endregion
    }
}