using IDesign.Recognizers.Abstractions;

namespace IDesign.Core
{
    public class RecognitionResult
    {
        public IResult Result { get; set; }
        public IEntityNode EntityNode { get; set; }
        public string FilePath { get; set; }
        public DesignPattern Pattern;
    }
}