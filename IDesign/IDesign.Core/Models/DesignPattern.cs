using IDesign.Recognizers.Abstractions;

namespace IDesign.Core.Models
{
    public class DesignPattern
    {
        public DesignPattern(string name, IRecognizer recognizer)
        {
            Name = name;
            Recognizer = recognizer;
        }

        public IRecognizer Recognizer { get; set; }
        public string Name { get; set; }
    }
}