using System;
using System.Runtime.Serialization;

namespace NPacMan.Bot
{
    [Serializable]
    internal class DeadPipeException : Exception
    {
        public DeadPipeException()
        {
        }

        public DeadPipeException(string? message) : base(message)
        {
        }

        public DeadPipeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DeadPipeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}