using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;

namespace Kuizlet.Serializers.Test
{
    public class JsonDataSerializerTests
    {
        private readonly JsonDataSerializer _serializer = new();

        [Fact]
        public void Serialize_ValidObject_ReturnsJsonString()
        {
            var flashcard = new Flashcard { Term = "Cat", Definition = "A small animal" };
            var json = _serializer.Serialize(flashcard);
            Assert.Contains("Cat", json);
            Assert.Contains("A small animal", json);
        }

        [Fact]
        public void Deserialize_ValidJson_ReturnsObject()
        {
            var json = "{\"Term\":\"Dog\",\"Definition\":\"A loyal animal\"}";
            var result = _serializer.Deserialize<Flashcard>(json);
            Assert.Equal("Dog", result.Term);
            Assert.Equal("A loyal animal", result.Definition);
        }
    }
    public class TxtDataSerializerTests
    {
        private readonly TxtDataSerializer _serializer = new();

        [Fact]
        public void Serialize_ListOfFlashcards_ReturnsText()
        {
            var cards = new List<Flashcard>
        {
            new Flashcard { Term = "Sun", Definition = "Star at the center" },
            new Flashcard { Term = "Moon", Definition = "Natural satellite" }
        };

            var txt = _serializer.Serialize(cards);
            Assert.Contains("Sun, Star at the center", txt);
            Assert.Contains("Moon, Natural satellite", txt);
        }

        [Fact]
        public void Deserialize_ValidText_ReturnsListOfFlashcards()
        {
            var txt = "Sun, Star at the center\nMoon, Natural satellite";
            var result = _serializer.Deserialize<List<Flashcard>>(txt);

            Assert.Equal(2, result.Count);
            Assert.Equal("Sun", result[0].Term);
            Assert.Equal("Star at the center", result[0].Definition);
        }
    }
    public class BasicTest
    {
        [Fact]
        public void SampleTest()
        {
            Assert.True(1 + 1 == 2);
        }
    }

    


    public class Flashcard
    {
        public string Term { get; set; }
        public string Definition { get; set; }
    }
}