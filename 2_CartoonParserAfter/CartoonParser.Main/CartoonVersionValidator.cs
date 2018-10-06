using CartoonParser.Main.Exceptions;

namespace CartoonParser.Main
{
    public class CartoonVersionValidator
    {
        public void Validate(string version)
        {
            var versionIsValid = version == CartoonVersion1Specification.CartoonVersion1;
            if (!versionIsValid)
            {
                throw new CartoonParserValidationException("Unable to read file. Unrecognized format.");
            }
        }
    }
}