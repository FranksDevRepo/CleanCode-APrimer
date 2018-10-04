using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CartoonParser.Main.Exceptions;
using NUnit.Framework;

namespace CartoonParser.Main.Tests
{
    [TestFixture]
    public class CartoonParserTests
    {
        private CartoonParser _parser;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _parser = new CartoonParser();
        }

        [Test]
        public void Parse_GivenV1Data_HasExpectedOnePunchManData()
        {
            var cartoonV1Data = GetAllLinesFromFile("SampleData/CartoonsV1.txt");

            var result = _parser.Parse(cartoonV1Data);
            var onePunchMan = result
                .FirstOrDefault(x => 
                    x.Name.Equals("One Punch Man", StringComparison.OrdinalIgnoreCase));
            
            Assert.IsNotNull(onePunchMan);
            Assert.AreEqual(DateTime.Parse("10/5/2015"), onePunchMan.ReleaseDate);
            Assert.AreEqual("Madhouse", onePunchMan.Studio);
            Assert.IsTrue(onePunchMan.Genres.Any(x => x.Equals("Action")));
            Assert.IsTrue(onePunchMan.Genres.Any(x => x.Equals("Comedy")));
            Assert.IsTrue(onePunchMan.Genres.Any(x => x.Equals("Superhero")));
        }

        [Test]
        public void Parse_GivenV1Data_HasExpectedNumberOfCartoons()
        {
            var cartoonV1Data = GetAllLinesFromFile("SampleData/CartoonsV1.txt");

            var result = _parser.Parse(cartoonV1Data);

            Assert.AreEqual(4, result.Count);
        }

        [Test]
        public void Parse_GivenDataWithInvalidSegmentCount_ThrowsExpectedException()
        {
            var cartoonData = GetAllLinesFromFile("SampleData/CartoonsV1_InvalidNumberOfSegments.txt");

            Assert.Throws<CartoonParserValidationException>(() => _parser.Parse(cartoonData));
        }

        [Test, TestCaseSource(nameof(_invalidFieldDataFiles))]
        public void Parse_GivenDataWithDataSegments_ThrowsExpectedException(string data)
        {
            Console.WriteLine(data);
            Assert.Throws<CartoonParserValidationException>(() => _parser.Parse(data));
        }

        private static IList<string> _invalidFieldDataFiles = new List<string>
        {
            GetAllLinesFromFile("SampleData/CartoonsV1_NoName.txt"),
            GetAllLinesFromFile("SampleData/CartoonsV1_NoReleaseDate.txt"),
            GetAllLinesFromFile("SampleData/CartoonsV1_NoStudio.txt"),
            GetAllLinesFromFile("SampleData/CartoonsV1_NoGenres.txt"),
            GetAllLinesFromFile("SampleData/CartoonsV1_DuplicateGenre.txt"),
            GetAllLinesFromFile("SampleData/CartoonsV1_EmptyGenre.txt")
        };

        private static string GetAllLinesFromFile(string v1DataRelativePath)
        {
            var v1DataPath = Path.Combine(TestContext.CurrentContext.TestDirectory, v1DataRelativePath);
            return File.ReadAllText(v1DataPath);
        }
    }
}
