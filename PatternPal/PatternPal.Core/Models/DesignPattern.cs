#region

using PatternPal.Protos;
using PatternPal.Recognizers.Abstractions;

#endregion

namespace PatternPal.Core.Models
{
    public class DesignPattern
    {
        public DesignPattern(
            string name,
            IRecognizer recognizer,
            string wikiPage,
            Recognizer recognizerType)
        {
            Name = name;
            Recognizer = recognizer;
            WikiPage = wikiPage;
            RecognizerType = recognizerType;
        }

        public IRecognizer Recognizer { get; }
        public string Name { get; }
        public string WikiPage { get; }
        public Recognizer RecognizerType { get; }
    }
}
