using IDesign.Recognizers.Abstractions;

namespace IDesign.Core.Models
{
    public class DesignPattern
    {
        public DesignPattern(string name, IRecognizer recognizer, string wikiPage)
        {
            Name = name;
            Recognizer = recognizer;
            WikiPage = wikiPage;
        }

        public IRecognizer Recognizer { get; set; }
        public string Name { get; set; }
        public string WikiPage { get; set; }
    }
}