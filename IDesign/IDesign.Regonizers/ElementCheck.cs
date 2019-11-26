using System;

namespace IDesign.Recognizers
{
    public partial class Recognizer
    {
        public class ElementCheck<T>
        {
            public Predicate<T> Check { get; set; }

            private string suggestionMessage;

            public string GetSuggestionMessage()
            {
                return suggestionMessage;
            }

            public void SetSuggestionMessage(string value)
            {
                suggestionMessage = value;
            }

            public ElementCheck(Predicate<T> check, string suggestionMessage)
            {
                Check = check;
                this.suggestionMessage = suggestionMessage;
            }
        }
    }
}
