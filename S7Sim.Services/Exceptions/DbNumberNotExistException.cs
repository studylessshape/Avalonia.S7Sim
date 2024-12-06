using System;

namespace S7Sim.Services.Exceptions
{

	[Serializable]
	public class DbNumberNotExistException : Exception
	{
		public int DbNumber { get; }
		public DbNumberNotExistException(int dbNumber)
		{
			DbNumber = dbNumber;
		}
		public DbNumberNotExistException(int dbNumber, string message) : base(message)
		{
			DbNumber = dbNumber;
		}
		public DbNumberNotExistException(int dbNumber, string message, Exception inner) : base(message, inner)
		{
			DbNumber = dbNumber;
		}
		protected DbNumberNotExistException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}