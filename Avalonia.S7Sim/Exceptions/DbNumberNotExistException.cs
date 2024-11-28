using System;

namespace Avalonia.S7Sim.Exceptions;

public class DbNumberNotExistException : Exception
{
    public int DbNumber { get; }

    public DbNumberNotExistException(int dbNumber)
    {
        DbNumber = dbNumber;
    }

    public DbNumberNotExistException(int dbNumber, string? message) : base(message)
    {
        DbNumber = dbNumber;
    }
}