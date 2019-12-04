using System;

namespace IDesign.Recognizers
{
    public partial class Recognizer
    {
        /// <summary>
        ///     A class for defining a check on a type with a predicate
        /// </summary>
        /// <typeparam name="T">The type witch the check is for</typeparam>
        public class ElementCheck<T>
        {
            private readonly string _suggestionMessage;

            public ElementCheck(Predicate<T> check, string suggestionMessage)
            {
                Check = check;
                _suggestionMessage = suggestionMessage;
            }

            public Predicate<T> Check { get; set; }

            public string GetSuggestionMessage()
            {
                return _suggestionMessage;
            }
        }
    }
}