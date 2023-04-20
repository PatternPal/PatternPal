using System.Collections.Generic;
using System.Linq;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Checks.Entities.GroupChecks;
using PatternPal.Recognizers.Models.Checks.Members;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Entities
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
            List<ICheckResult> results = _checks.Select(c => c.Check(elementToCheck)).ToList();

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

        /// <inheritdoc cref="AbstractMemberListCheck{T,R}.Modifiers"/>
        public EntityCheck Modifiers(params IModifier[] modifiers)
        {
            _checks.Add(new ModifierCheck(modifiers));
            return This();
        }
        
        public EntityCheck Type(EntityType type)
        {
            return Custom(e => e.GetEntityType() == type, new ResourceMessage("EntityType", type.ToString()));
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
