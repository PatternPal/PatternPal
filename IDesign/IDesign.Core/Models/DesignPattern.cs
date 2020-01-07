using IDesign.Recognizers.Abstractions;

namespace IDesign.Core.Models
{
    public class DesignPattern
    {
        public DesignPattern(string name, IRecognizer recognizer, string wikilink)
        {
            Name = name;
            Recognizer = recognizer;
            WikiLink = wikilink;
        }

        public IRecognizer Recognizer { get; set; }
        public string Name { get; set; }
        public string WikiLink { get; set; }
    }
}