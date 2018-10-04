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
        /// <summary>
        /// Parses cartoon data from a single format. All fields are required.
        /// </summary>
        /// <param name="data">The data</param>
        /// <returns>A list of cartoons</returns>
        public IList<Cartoon> Parse(string data)
        {
            /* Beginning */
            var cartoons = new List<Cartoon>();
            using (var stringReader = new StringReader(data))
            {
                /* Line Parsing */
                string row;
                var l = 0;
                string vNoDec = null;
                while ((row = stringReader.ReadLine()) != null)
                {
                    /* Version number check */
                    if (l == 0)
                    {
                        vNoDec = row.Split('|')[1];
                        // JR: Removed version 2.0 format validation
                        if (row.Split('|')[1] != "1.0"/* && row.Split('|')[1] != "2.0"*/)
                        {
                            throw new CartoonParserValidationException("Unable to read file. Unrecognized format.");
                        }
                    }
                    /* Actual line parsing */
                    else
                    {
                        switch (vNoDec)
                        {
                            // Handle version 1 file lines
                            case "1.0":
                                // Validate the data
                                if (row.Split('|').Length == 4)
                                {
                                    if (!string.IsNullOrWhiteSpace(row.Split('|')[0]))
                                    {
                                        if (DateTime.TryParse(row.Split('|')[1], out _))
                                        {
                                            if (!string.IsNullOrWhiteSpace(row.Split('|')[2]))
                                            {
                                                if (!string.IsNullOrWhiteSpace(row.Split('|')[3]) && row.Split('|')[3].Split(',').All(x => !string.IsNullOrEmpty(x)) && row.Split('|')[3].Split(',').GroupBy(x => x).All(x => x.Count() == 1))
                                                {
                                                    // Map the values
                                                    var nc = new Cartoon
                                                    {
                                                        Name = row.Split('|')[0],
                                                        ReleaseDate = DateTime.Parse(row.Split('|')[1]),
                                                        Studio = row.Split('|')[2],
                                                        Genres = row.Split('|')[3].Split(',')
                                                    };

                                                    // add the cartoon
                                                    cartoons.Add(nc);
                                                }
                                                else
                                                {
                                                    // Throw validation exception on genre
                                                    throw new CartoonParserValidationException("Invalid Cartoon Genre List detected. Unable to parse file.");
                                                } // End else for genre validation
                                            }
                                            else
                                            {
                                                // Throw validation exception on studio
                                                throw new CartoonParserValidationException("Invalid Cartoon Studio detected. Unable to parse file.");
                                            } // End else for studio validation
                                        }
                                        else
                                        {
                                            // Throw validation exception on release date
                                            throw new CartoonParserValidationException("Invalid Cartoon Release Date detected. Unable to parse file.");
                                        } // End else for release date validation
                                    }
                                    else
                                    {
                                        // Throw validation exception on name
                                        throw new CartoonParserValidationException("Invalid Cartoon Name detected. Unable to parse file.");
                                    } // End else for name validation
                                }
                                else
                                {
                                    // Throw validation exception on data length
                                    throw new CartoonParserValidationException("Incorrect number of segments in row data. Unable to parse file.");
                                } // End else for data length
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
                    } // End Else for non-version line
                    // increment the line number
                    l++;
                }
            }

            return cartoons;
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
