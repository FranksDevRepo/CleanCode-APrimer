using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CartoonParser.Main.Exceptions;
using static CartoonParser.Main.CartoonVersion1Specification;

// TODO: Clean this code!
namespace CartoonParser.Main
{
    public class CartoonMapper
    {
        public Cartoon Map(string line)
        {
            return new Cartoon
            {
                Name = line.Split(SegmentDelimiter)[SegmentIndexName],
                ReleaseDate = DateTime.Parse(line.Split(SegmentDelimiter)[SegmentIndexReleaseDate]),
                Studio = line.Split(SegmentDelimiter)[SegmentIndexStudio],
                Genres = line.Split(SegmentDelimiter)[SegmentIndexGenres].Split(GenreDelimiter)
            };
        }
    }

    public static class CartoonVersion1Specification
    {
        public const int ValidSegmentLength = 4;
        public const int SegmentIndexName = 0;
        public const int SegmentIndexReleaseDate = 1;
        public const int SegmentIndexStudio = 2;
        public const int SegmentIndexGenres = 3;
        public const string CartoonVersion1 = "1.0";
        public const char SegmentDelimiter = '|';
        public const char GenreDelimiter = ',';
    }

    public class CartoonParser
    {
        private readonly CartoonMapper _mapper;
        private List<Cartoon> _cartoons;
        private string _version;

        public CartoonParser(CartoonMapper mapper)
        {
            _mapper = mapper;
        }

        public IList<Cartoon> Parse(string text)
        {
            _cartoons = new List<Cartoon>();

            var allLines = GetAllLines(text);

            _version = GetVersion(allLines);
            ValidateVersion(_version);

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

        private static void ValidateVersion(string version)
        {
            var versionIsValid = version == CartoonVersion1Specification.CartoonVersion1;
            if (!versionIsValid)
            {
                throw new CartoonParserValidationException("Unable to read file. Unrecognized format.");
            }
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
            Validate(line);
            var cartoon = _mapper.Map(line);
            _cartoons.Add(cartoon);
        }

        private static void Validate(string line)
        {
            var segmentLengthIsValid = line.Split(CartoonVersion1Specification.SegmentDelimiter).Length == CartoonVersion1Specification.ValidSegmentLength;
            if (!segmentLengthIsValid)
            {
                throw new CartoonParserValidationException(
                    "Incorrect number of segments in row data. Unable to parse file.");
            }

            var nameIsValid = !string.IsNullOrWhiteSpace(line.Split(CartoonVersion1Specification.SegmentDelimiter)[0]);
            if (!nameIsValid)
            {
                throw new CartoonParserValidationException("Invalid Cartoon Name detected. Unable to parse file.");
            }

            var releaseDateIsValid = DateTime.TryParse(line.Split(CartoonVersion1Specification.SegmentDelimiter)[CartoonVersion1Specification.SegmentIndexReleaseDate], out _);
            if (!releaseDateIsValid)
            {
                throw new CartoonParserValidationException(
                    "Invalid Cartoon Release Date detected. Unable to parse file.");
            }

            var studioIsValid = !string.IsNullOrWhiteSpace(line.Split(CartoonVersion1Specification.SegmentDelimiter)[CartoonVersion1Specification.SegmentIndexStudio]);
            if (!studioIsValid)
            {
                throw new CartoonParserValidationException(
                    "Invalid Cartoon Studio detected. Unable to parse file.");
            }

            if (!GenreIsValid(line))
            {
                throw new CartoonParserValidationException(
                    "Invalid Cartoon Genre List detected. Unable to parse file.");
            }
        }

        private static bool GenreIsValid(string line)
        {
            var genresSegmentIsEmpty = string.IsNullOrWhiteSpace(line.Split(CartoonVersion1Specification.SegmentDelimiter)[CartoonVersion1Specification.SegmentIndexGenres]);
            var oneGenreIsEmpty = !line.Split(CartoonVersion1Specification.SegmentDelimiter)[CartoonVersion1Specification.SegmentIndexGenres].Split(CartoonVersion1Specification.GenreDelimiter)
                .All(x => !string.IsNullOrEmpty(x));
            var hasDuplicateGenre = !line.Split(CartoonVersion1Specification.SegmentDelimiter)[CartoonVersion1Specification.SegmentIndexGenres]
                .Split(CartoonVersion1Specification.GenreDelimiter).GroupBy(x => x)
                .All(x => x.Count() == CartoonVersion1Specification.SegmentIndexReleaseDate);

            return !genresSegmentIsEmpty &&
                   !oneGenreIsEmpty &&
                   !hasDuplicateGenre;
        }
    }

    public class Cartoon
    {
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Studio { get; set; }
        public IList<string> Genres { get; set; }
    }
}
