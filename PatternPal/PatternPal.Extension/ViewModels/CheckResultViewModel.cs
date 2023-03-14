#region

using System.Collections.Generic;
using System.Linq;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.ViewModels
{
    public class CheckResultViewModel
    {
        public CheckResult Result { get; set; }
        public string Message { get; set; }
        public float Score => (float)Result.Score;
        public FeedbackType ChildrenFeedbackType { get; set; }
        public IEnumerable< CheckResultViewModel > SubResults => GetSubResults();

        public CheckResultViewModel(
            CheckResult result,
            FeedbackType childrenFeedbackType = FeedbackType.FeedbackCorrect)
        {
            Result = result;
            Message = Result.FeedbackMessage;
            ChildrenFeedbackType = childrenFeedbackType;
        }

        /// <summary>
        /// Loops through all subresults recursively and place them in an IEnumberable
        /// </summary>
        /// <returns>IEnumerable that contains CheckResultViewModel classes</returns>
        private IEnumerable< CheckResultViewModel > GetSubResults()
        {
            List< CheckResultViewModel > toReturn = new List< CheckResultViewModel >();

            foreach (CheckResult result in Result.ChildFeedback)
            {
                GetSubResultsRecursive(
                    toReturn,
                    result);
            }

            return toReturn;
        }

        /// <summary>
        /// Recursive function, used by GetSubResults
        /// </summary>
        /// <param name="toReturn"></param>
        /// <param name="result"></param>
        private void GetSubResultsRecursive(
            List< CheckResultViewModel > toReturn,
            CheckResult result)
        {
            if (result.Hidden)
            {
                foreach (CheckResult sub in result.ChildFeedback)
                {
                    GetSubResultsRecursive(
                        toReturn,
                        sub);
                }
            }
            else
                if (!result.ChildFeedback.Any()
                    && result.FeedbackType == ChildrenFeedbackType)
                {
                    toReturn.Add(new CheckResultViewModel(result));
                }
        }
    }
}
