using System;
using System.Collections.Generic;
using System.Linq;
using CartoonParser.Main.Exceptions;
using CartoonParser.Main.Validation;

// TODO: Clean this code!
namespace CartoonParser.Main
{
    public class CartoonParser
    {
        private readonly CartoonMapper _mapper;
        private readonly CartoonValidator _validator;
        private readonly CartoonVersionValidator _versionValidator;
        private List<Cartoon> _cartoons;
        private string _version;

        public CartoonParser(CartoonMapper mapper, CartoonValidator validator, CartoonVersionValidator versionValidator)
        {
            _mapper = mapper;
            _validator = validator;
            _versionValidator = versionValidator;
        }

        public IList<Cartoon> Parse(string text)
        {
            _cartoons = new List<Cartoon>();

            var allLines = GetAllLines(text);

            _version = GetVersion(allLines);
            _versionValidator.Validate(_version);

            var linesToParse = GetLinesToParse(allLines);
            ParseLines(linesToParse);

            return _cartoons;
        }

        private static List<string> GetAllLines(string text)
        {
            return text
                .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        private static string GetVersion(List<string> allLines)
        {
            return allLines
                .Take(1)
                .FirstOrDefault()
                ?.Split(CartoonVersion1Specification.SegmentDelimiter)
                .LastOrDefault();
        }

        private static List<string> GetLinesToParse(List<string> allLines)
        {
            return allLines.Skip(1).ToList();
        }

        private void ParseLines(List<string> linesToParse)
        {
            foreach (var line in linesToParse)
            {
                ParseLine(line);
            }
        }

        private void ParseLine(string line)
        {
            switch (_version)
            {
                case CartoonVersion1Specification.CartoonVersion1:
                    ValidateAndMapAndAdd(line);
                    break;
                default:
                    throw new CartoonParserValidationException("No version detected. Unable to parse.");
            }
        }

        private void ValidateAndMapAndAdd(string line)
        {
            _validator.Validate(line);
            var cartoon = _mapper.Map(line);
            _cartoons.Add(cartoon);
        }
    }
}
