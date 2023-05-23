using System.Collections.Generic;
using System.Linq;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Output;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Entities.GroupChecks
{
    public class AllMemberGroupCheck : MemberGroupCheck
    {
        public AllMemberGroupCheck(ICheckBuilder<IEntity> root) : base(root)
        {
        }

        public override ICheckResult Check(Dictionary<IMember, List<ICheckResult>> results, IMemberCheckBuilder check)
        {
            throw new System.NotImplementedException();
        }
    }
}
