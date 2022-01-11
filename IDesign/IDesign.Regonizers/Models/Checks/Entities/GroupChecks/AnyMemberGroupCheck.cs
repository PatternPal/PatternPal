using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities.GroupChecks
{
    public class AnyMemberGroupCheck : MemberGroupCheck
    {
        public AnyMemberGroupCheck(ICheckBuilder<IEntity> root) : base(root)
        {
        }

        public override ICheckResult Check(Dictionary<IMember, List<ICheckResult>> results, IMemberCheckBuilder check)
        {
            var any = results.FirstOrDefault(pair => pair.Value.All(r => r.GetScore() != 0));
            if (any.Key == null)
            {
                var best = results
                    .OrderBy(p => -p.Value.Count(r => r.GetScore() != 0))
                    .FirstOrDefault();

                var result = new CheckResult(check.GetMessage(), FeedbackType.Incorrect, best.Key)
                {
                    ChildFeedback = best.Value ?? new List<ICheckResult>()
                };
                return result;
            }
            else
            {
                var result =
                    new CheckResult(check.GetMessage(), FeedbackType.Correct, any.Key)
                    {
                        ChildFeedback = any.Value ?? new List<ICheckResult>()
                    };
                return result;
            }
        }
    }
}
