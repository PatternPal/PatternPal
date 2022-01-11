using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Checks.Entities.GroupChecks;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities
{
    public class EntityCheck : AbstractListCheck<IEntity, EntityCheck>, ICheckBuilder<IEntity>, ICheck<IEntity>
    {
        public EntityCheck()
        {
            All = new AllMemberGroupCheck(this);
            Any = new AnyMemberGroupCheck(this);

            Groups = new MemberGroupCheck[] { All, Any };
        }

        protected override EntityCheck This() => this;

        public AnyMemberGroupCheck Any { get; }
        public AllMemberGroupCheck All { get; }
        public EntityCheck Build() => this;

        protected readonly MemberGroupCheck[] Groups;

        public ICheckResult Check(IEntity elementToCheck)
        {
            List<ICheckResult> results = new List<ICheckResult>();
            foreach (var group in Groups)
            {
                foreach (var check in group)
                {
                    List<IMember> members = check.GetCheckable(elementToCheck).ToList();
                    
                    Dictionary<IMember, List<ICheckResult>> resultsD = members.ToDictionary(
                        member => member,
                        member => check.Checks().Select(c => c.Check(member)).ToList()
                    );
                    results.Add(group.Check(resultsD, check));
                }
            }

            var result = new CheckResult("", FeedbackType.Correct, elementToCheck) { ChildFeedback = results };
            return result;
        }

        public IResult ToResult(IEntity entity)
        {
            var result = new Result();

            var r = Check(entity);
            result.Results = r.GetChildFeedback().ToList();

            return result;
        }
    }

    public interface ICheckBuilder<T> where T : INode
    {
        /// <summary>
        /// Any member should match
        /// </summary>
        AnyMemberGroupCheck Any { get; }

        /// <summary>
        /// All members of that type should match
        /// </summary>
        AllMemberGroupCheck All { get; }

        /// <summary>
        ///     Builds the check
        /// </summary>
        /// <returns>The root object</returns>
        EntityCheck Build();
    }
}
