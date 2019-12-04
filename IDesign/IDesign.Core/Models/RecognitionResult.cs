using IDesign.Recognizers.Abstractions;

namespace IDesign.Core
{
    public class RecognitionResult
    {
        public DesignPattern Pattern;
        public IResult Result { get; set; }
        public IEntityNode EntityNode { get; set; }
        public string FilePath { get; set; }
    }
}