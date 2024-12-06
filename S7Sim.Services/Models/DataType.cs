using System;

namespace S7Sim.Services.Models
{
    [Flags]
    public enum DataType
    {
        Bit = 0x00001,
        SInt = 0x00002,
        Int = 0x00004,
        DInt = 0x00008,
        LInt = 0x00010,
        USInt = 0x00020,
        UDInt = 0x00080,
        Byte = 0x00100,
        Word = 0x00200,
        DWord = 0x00400,
        LWord = 0x00800,
        Real = 0x01000,
        LReal = 0x02000,
        // DateTime = 0x04000,
        // Date = 0x08000,
        // TOD = 0x010000,
        // LTOD = 0x020000,
        // LDT = 0x040000,
        // DTL = 0x060000,
        // String = 0x080000,
        // Array = 0x100000,
    }
}