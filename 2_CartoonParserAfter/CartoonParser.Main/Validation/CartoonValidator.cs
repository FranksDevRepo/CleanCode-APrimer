using System;
using System.Linq;
using CartoonParser.Main.Exceptions;

namespace CartoonParser.Main.Validation
{
    public class CartoonValidator
    {
        public void Validate(string line)
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
}