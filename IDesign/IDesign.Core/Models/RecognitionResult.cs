using IDesign.Recognizers.Abstractions;
using SyntaxTree.Abstractions;

namespace IDesign.Core.Models
{
    public class RecognitionResult
    {
        public DesignPattern Pattern;
        public IResult Result { get; set; }
        public IEntityNode EntityNode { get; set; }
        public string FilePath { get; set; }
    }
}