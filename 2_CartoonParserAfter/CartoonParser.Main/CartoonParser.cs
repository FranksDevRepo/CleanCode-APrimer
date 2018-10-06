using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CartoonParser.Main.Exceptions;

// TODO: Clean this code!
namespace CartoonParser.Main
{
    public class CartoonParser
    {
        private List<Cartoon> _cartoons;
        private string _version;
        private const int ValidSegmentLength = 4;
        private const int SegmentIndexName = 0;
        private const int SegmentIndexReleaseDate = 1;
        private const int SegmentIndexStudio = 2;
        private const int SegmentIndexGenres = 3;
        private const string CartoonVersion1 = "1.0";
        private const char SegmentDelimiter = '|';
        private const char GenreDelimiter = ',';

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
                ?.Split(SegmentDelimiter)
                .LastOrDefault();
        }

        private static void ValidateVersion(string version)
        {
            var versionIsValid = version == CartoonVersion1;
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
                case CartoonVersion1:
                    ValidateAndMapAndAdd(line);
                    break;
                default:
                    throw new CartoonParserValidationException("No version detected. Unable to parse.");
            }
        }

        private void ValidateAndMapAndAdd(string line)
        {
            Validate(line);
            var cartoon = Map(line);
            _cartoons.Add(cartoon);
        }

        private static void Validate(string line)
        {
            var segmentLengthIsValid = line.Split(SegmentDelimiter).Length == ValidSegmentLength;
            if (!segmentLengthIsValid)
            {
                throw new CartoonParserValidationException(
                    "Incorrect number of segments in row data. Unable to parse file.");
            }

            var nameIsValid = !string.IsNullOrWhiteSpace(line.Split(SegmentDelimiter)[0]);
            if (!nameIsValid)
            {
                throw new CartoonParserValidationException("Invalid Cartoon Name detected. Unable to parse file.");
            }

            var releaseDateIsValid = DateTime.TryParse(line.Split(SegmentDelimiter)[SegmentIndexReleaseDate], out _);
            if (!releaseDateIsValid)
            {
                throw new CartoonParserValidationException(
                    "Invalid Cartoon Release Date detected. Unable to parse file.");
            }

            var studioIsValid = !string.IsNullOrWhiteSpace(line.Split(SegmentDelimiter)[SegmentIndexStudio]);
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
            var genresSegmentIsEmpty = string.IsNullOrWhiteSpace(line.Split(SegmentDelimiter)[SegmentIndexGenres]);
            var oneGenreIsEmpty = !line.Split(SegmentDelimiter)[SegmentIndexGenres].Split(GenreDelimiter)
                .All(x => !string.IsNullOrEmpty(x));
            var hasDuplicateGenre = !line.Split(SegmentDelimiter)[SegmentIndexGenres]
                .Split(GenreDelimiter).GroupBy(x => x)
                .All(x => x.Count() == SegmentIndexReleaseDate);

            return !genresSegmentIsEmpty &&
                   !oneGenreIsEmpty &&
                   !hasDuplicateGenre;
        }

        private Cartoon Map(string line)
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

    public class Cartoon
    {
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Studio { get; set; }
        public IList<string> Genres { get; set; }
    }
}
