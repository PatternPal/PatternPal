using System.Collections.Generic;
using System.Linq;

using PatternPal.CommonResources;
using PatternPal.Protos;
using PatternPal.Recognizers.Abstractions;

using FeedbackType = PatternPal.Recognizers.Abstractions.FeedbackType;

namespace PatternPal.Extension.ViewModels
{
    public class CheckResultViewModel
    {
        public CheckResult Result { get; set; }
        public string Message { get; set; }
        public float Score => (float)Result.Score;
        public Protos.FeedbackType ChildrenFeedbackType { get; set; }
        public IEnumerable< CheckResultViewModel > SubResults => GetSubResults();

        public CheckResultViewModel(
            ICheckResult result,
            FeedbackType childrenFeedbackType)
        {
            //Result = result;
            //Message = ResourceUtils.ResultToString(Result);
            //ChildrenFeedbackType = childrenFeedbackType;
        }

        public CheckResultViewModel(
            ICheckResult result)
            : this(
                result,
                FeedbackType.Correct)
        {
        }

        public CheckResultViewModel(
            CheckResult result,
            Protos.FeedbackType childrenFeedbackType)
        {
            Result = result;
            ChildrenFeedbackType = childrenFeedbackType;
        }

        public CheckResultViewModel(
            CheckResult result)
            : this(
                result,
                Protos.FeedbackType.FeedbackCorrect)
        {
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
