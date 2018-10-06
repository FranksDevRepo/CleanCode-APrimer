using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CartoonParser.Main.Exceptions;

// TODO: Clean this code!
/*************************************************************************************************
 * Introducing the Cartoon Parser!
 * This parser was written by some guy who is no longer supporting it.
 * It's our responsibility now.
 * This parser accepts a single format and will return a list of cartoons with Name, Release Date, Studio, and Genres.
 * This parser may be too complicated to change. It's best not to change it...
*************************************************************************************************/
namespace CartoonParser.Main
{
    /// <summary>
    /// The cartoon parser
    /// </summary>
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

        /// <summary>
        /// Parses cartoon text from a single format. All fields are required.
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>A list of cartoons</returns>
        public IList<Cartoon> Parse(string text)
        {
            /* Beginning */
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
            // JR: Removed version 2.0 format validation
            if (version != CartoonVersion1 /* && row.Split('|')[1] != "2.0"*/)
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
            /* Version number check */
            switch (_version)
            {
                // Handle version 1 file lines
                case CartoonVersion1:
                    // Validate the data
                    ValidateAndMapAndAdd(line);
                    break;
                // Handle version 2 file lines
                //case "2.0":
                //// Validate the data
                //if (row.Split('|').Length == 5)
                //{
                //    // File must have valid name
                //    if (!string.IsNullOrWhiteSpace(row.Split('|')[0]))
                //    {
                //        // File must have valid release date
                //        if (DateTime.TryParse(row.Split('|')[1], out var _))
                //        {
                //            // File must have valid studio
                //            if (!string.IsNullOrWhiteSpace(row.Split('|')[2]))
                //            {
                //                // File must have non-empty genre list with non-empty entries
                //                if (!string.IsNullOrWhiteSpace(row.Split('|')[3]) && row.Split('|')[3].Split(',').All(x => !string.IsNullOrEmpty(x)) && row.Split('|')[3].Split(',').GroupBy(x => x).Any(x => x.Any()))
                //                {
                //                    // File must have valid IMDB score
                //                    if (decimal.TryParse(row.Split('|')[4], out var _))
                //                    {
                //                        // add the cartoon
                //                        cartoons.Add(new Cartoon
                //                        {
                //                            //Map the values
                //                            Name = row.Split('|')[0],
                //                            ReleaseDate = DateTime.Parse(row.Split('|')[1]),
                //                            Studio = row.Split('|')[2],
                //                            Genres = row.Split('|')[3].Split(','),
                //                            ImdbScore = Convert.ToDecimal(row.Split('|')[4])
                //                        });
                //                    }
                //                    else
                //                    {
                //                        // Throw validation exception on genre
                //                        throw new CartoonParserValidationException("Invalid Cartoon IMDB Score detected. Unable to parse file.");
                //                    } // End else for genre validation
                //                }
                //                else
                //                {
                //                    // Throw validation exception on genre
                //                    throw new CartoonParserValidationException("Invalid Cartoon Genre List detected. Unable to parse file.");
                //                } // End else for genre validation
                //            }
                //            else
                //            {
                //                // Throw validation exception on studio
                //                throw new CartoonParserValidationException("Invalid Cartoon Studio detected. Unable to parse file.");
                //            } // End else for studio validation
                //        }
                //        else
                //        {
                //            // Throw validation exception on release date
                //            throw new CartoonParserValidationException("Invalid Cartoon Release Date detected. Unable to parse file.");
                //        } // End else for release date validation
                //    }
                //    else
                //    {
                //        // Throw validation exception on name
                //        throw new CartoonParserValidationException("Invalid Cartoon Name detected. Unable to parse file.");
                //    } // End else for name validation
                //}
                //else
                //{
                //    // Throw validation exception on data length
                //    throw new CartoonParserValidationException("Incorrect number of segments in row data. Unable to parse file.");
                //} // End else for data length
                //break;
                default:
                    throw new CartoonParserValidationException("No version detected. Unable to parse.");
            }

            // End Else for non-version line

            // increment the line number
        }

        private void ValidateAndMapAndAdd(string line)
        {
            if (line.Split(SegmentDelimiter).Length != ValidSegmentLength)
            {
                // Throw validation exception on data length
                throw new CartoonParserValidationException(
                    "Incorrect number of segments in row data. Unable to parse file.");
            } // End else for data length

            if (string.IsNullOrWhiteSpace(line.Split(SegmentDelimiter)[0]))
            {
                // Throw validation exception on name
                throw new CartoonParserValidationException("Invalid Cartoon Name detected. Unable to parse file.");
            } // End else for name validation

            if (!DateTime.TryParse(line.Split(SegmentDelimiter)[SegmentIndexReleaseDate], out _))
            {
                // Throw validation exception on release date
                throw new CartoonParserValidationException(
                    "Invalid Cartoon Release Date detected. Unable to parse file.");
            } // End else for release date validation

            if (string.IsNullOrWhiteSpace(line.Split(SegmentDelimiter)[SegmentIndexStudio]))
            {
                // Throw validation exception on studio
                throw new CartoonParserValidationException(
                    "Invalid Cartoon Studio detected. Unable to parse file.");
            } // End else for studio validation

            if (string.IsNullOrWhiteSpace(line.Split(SegmentDelimiter)[SegmentIndexGenres]) ||
                !line.Split(SegmentDelimiter)[SegmentIndexGenres].Split(GenreDelimiter)
                    .All(x => !string.IsNullOrEmpty(x)) ||
                !line.Split(SegmentDelimiter)[SegmentIndexGenres]
                    .Split(GenreDelimiter).GroupBy(x => x)
                    .All(x => x.Count() == SegmentIndexReleaseDate))
            {
                // Throw validation exception on genre
                throw new CartoonParserValidationException(
                    "Invalid Cartoon Genre List detected. Unable to parse file.");
            } // End else for genre validation

            // Map the values
            var cartoon = Map(line);

            // add the cartoon
            _cartoons.Add(cartoon);
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
        // Version 1 Data
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Studio { get; set; }
        public IList<string> Genres { get; set; }
        // Version 2 data
        //public decimal ImdbScore { get; set; }
    }
}
