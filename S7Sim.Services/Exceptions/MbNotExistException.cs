using System;

namespace S7Sim.Services.Exceptions
{

    [Serializable]
    public class MbNotExistException : Exception
    {
        public MbNotExistException() { }
        public MbNotExistException(string message) : base(message) { }
        public MbNotExistException(string message, Exception inner) : base(message, inner) { }
        protected MbNotExistException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
