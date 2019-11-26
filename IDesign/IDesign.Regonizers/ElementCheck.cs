using System;

namespace IDesign.Recognizers
{
    public partial class Recognizer
    {
        public class ElementCheck<T>
        {
            private string suggestionMessage;

            public ElementCheck(Predicate<T> check, string suggestionMessage)
            {
                Check = check;
                this.suggestionMessage = suggestionMessage;
            }

            public Predicate<T> Check { get; set; }

            public string GetSuggestionMessage()
            {
                return suggestionMessage;
            }

            public void SetSuggestionMessage(string value)
            {
                suggestionMessage = value;
            }
        }
    }
}