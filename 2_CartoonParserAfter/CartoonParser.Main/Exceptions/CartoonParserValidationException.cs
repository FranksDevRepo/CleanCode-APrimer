using System;

namespace CartoonParser.Main.Exceptions
{
    public class CartoonParserValidationException : Exception
    {
        public CartoonParserValidationException() { }

        public CartoonParserValidationException(string message) : base(message) { }

        public CartoonParserValidationException(string message, Exception innerException) : base(message, innerException) {}
    }
}