using IDesign.Regonizers.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Regonizers.Output
{
    public class Result : IResult
    {
        public List<ISuggestion> Suggestions { get; set; } = new List<ISuggestion>();
        public int Score { get; set; }

        public int GetScore()
        {
            return Score;
        }

        public IList<ISuggestion> GetSuggestions()
        {
            return Suggestions;
        }
    }


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
