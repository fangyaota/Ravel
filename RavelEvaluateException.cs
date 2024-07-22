using System.Runtime.Serialization;

namespace Ravel
{
    internal class RavelEvaluateException : Exception
    {
        public RavelEvaluateException()
        {
        }

        public RavelEvaluateException(string? message) : base(message)
        {
        }

        public RavelEvaluateException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected RavelEvaluateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
