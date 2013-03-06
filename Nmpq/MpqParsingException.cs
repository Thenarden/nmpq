using System;

namespace Nmpq
{
    public class MpqParsingException : Exception
    {
        public MpqParsingException(string message) : base(message)
        {
        }
    }
}