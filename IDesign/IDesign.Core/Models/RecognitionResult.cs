using IDesign.Recognizers.Abstractions;
using SyntaxTree.Abstractions.Entities;

namespace IDesign.Core.Models
{
    public class RecognitionResult
    {
        public DesignPattern Pattern;
        public IResult Result { get; set; }
        public IEntity EntityNode { get; set; }
        public string FilePath { get; set; }
    }
}
