using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Checks.Members;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities
{
    public abstract class MemberGroupCheck : IEnumerable<IMemberCheckBuilder>
    {
        private readonly ICheckBuilder<IEntity> _root;
        private readonly List<IMemberCheckBuilder> _builders = new List<IMemberCheckBuilder>();
        protected MemberGroupCheck(ICheckBuilder<IEntity> root) { _root = root; }

        public IEnumerator<IMemberCheckBuilder> GetEnumerator() => _builders.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public CustomMemberCheckBuilder<T, N> Custom<T, N>(N check) where T : IMember where N : AbstractMemberListCheck<T, N>
        {
            var builder = new CustomMemberCheckBuilder<T, N>(_root, check);
            _builders.Add(builder);
            return builder;
        }

        public MethodCheckBuilder Method(bool all = false)
        {
            var builder = new MethodCheckBuilder(_root, all);
            _builders.Add(builder);
            return builder;
        }

        public FieldCheckBuilder Field(bool all = false)
        {
            var builder = new FieldCheckBuilder(_root, all);
            _builders.Add(builder);
            return builder;
        }

        public ConstructorCheckBuilder Constructor()
        {
            var builder = new ConstructorCheckBuilder(_root);
            _builders.Add(builder);
            return builder;
        }

        public PropertyCheckBuilder Property()
        {
            var builder = new PropertyCheckBuilder(_root);
            _builders.Add(builder);
            return builder;
        }

        public abstract ICheckResult Check(Dictionary<IMember, List<ICheckResult>> results, IMemberCheckBuilder check);
    }

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
                var result = new CheckResult("any", FeedbackType.Incorrect, null)
                {
                    ChildFeedback = results
                        .OrderBy(p => -p.Value.Count(r => r.GetScore() != 0))
                        .Select(p => p.Value)
                        .FirstOrDefault() ?? new List<ICheckResult>()
                };
                return result;
            }
            else
            {
                var result = new CheckResult("any", FeedbackType.Correct, any.Key) { ChildFeedback = any.Value ?? new List<ICheckResult>() };
                return result;
            }
        }
    }

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
