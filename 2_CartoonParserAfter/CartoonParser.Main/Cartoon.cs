using System;
using System.Collections.Generic;

namespace CartoonParser.Main
{
    public class Cartoon
    {
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Studio { get; set; }
        public IList<string> Genres { get; set; }
    }
}