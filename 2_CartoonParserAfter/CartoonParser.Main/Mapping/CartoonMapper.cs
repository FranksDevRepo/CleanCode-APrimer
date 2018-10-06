using System;

namespace CartoonParser.Main
{
    public class CartoonMapper
    {
        public Cartoon Map(string line)
        {
            return new Cartoon
            {
                Name = line.Split(CartoonVersion1Specification.SegmentDelimiter)[CartoonVersion1Specification.SegmentIndexName],
                ReleaseDate = DateTime.Parse(line.Split(CartoonVersion1Specification.SegmentDelimiter)[CartoonVersion1Specification.SegmentIndexReleaseDate]),
                Studio = line.Split(CartoonVersion1Specification.SegmentDelimiter)[CartoonVersion1Specification.SegmentIndexStudio],
                Genres = line.Split(CartoonVersion1Specification.SegmentDelimiter)[CartoonVersion1Specification.SegmentIndexGenres].Split(CartoonVersion1Specification.GenreDelimiter)
            };
        }
    }
}