using PatternPal.Recognizers.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.Core.Models
{
    public class RecognitionResult
    {
        public DesignPattern Pattern;
        public IResult Result { get; set; }
        public IEntity EntityNode { get; set; }
        public string FilePath { get; set; }
    }
}
