using IDesign.Recognizers.Abstractions;

namespace IDesign.Core
{
    public class DesignPattern
    {
        public string Name { get; set; }
        public IRecognizer Recognizer { get; set; }

        public DesignPattern(string name, IRecognizer recognizer)
        {
            Name = name;
            Recognizer = recognizer;
        }
    }
}