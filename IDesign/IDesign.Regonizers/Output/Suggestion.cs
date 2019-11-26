using IDesign.Recognizers.Abstractions;

namespace IDesign.Recognizers.Output
{
    public class Suggestion : ISuggestion
    {
        public Suggestion(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        public string GetMessage()
        {
            return Message;
        }
    }
}