using System.Collections;
using System.Collections.Generic;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Checks.Entities.MemberBuilders;
using PatternPal.Recognizers.Models.Checks.Members;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Entities.GroupChecks
{
    public abstract class MemberGroupCheck : IEnumerable<IMemberCheckBuilder>
    {
        private readonly ICheckBuilder<IEntity> _root;
        private readonly List<IMemberCheckBuilder> _builders = new List<IMemberCheckBuilder>();
        protected MemberGroupCheck(ICheckBuilder<IEntity> root) { _root = root; }

        public IEnumerator<IMemberCheckBuilder> GetEnumerator() => _builders.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public CustomMemberCheckBuilder<T, N> Custom<T, N>(N check, string message = "")
            where T : IMember where N : AbstractMemberListCheck<T, N>
        {
            return Custom<T, N>(check, new ResourceMessage(message));
        }

        public CustomMemberCheckBuilder<T, N> Custom<T, N>(N check, ResourceMessage message)
            where T : IMember where N : AbstractMemberListCheck<T, N>
        {
            var builder = new CustomMemberCheckBuilder<T, N>(_root, check, message);
            _builders.Add(builder);
            return builder;
        }

        public MethodCheckBuilder Method(string message = "", bool variants = false)
        {
            return Method(new ResourceMessage(message), variants);
        }

        public MethodCheckBuilder Method(ResourceMessage message, bool variants = false)
        {
            var builder = new MethodCheckBuilder(_root, message, variants);
            _builders.Add(builder);
            return builder;
        }

        public FieldCheckBuilder Field(string message = "", bool variants = false)
        {
            return Field(new ResourceMessage(message), variants);
        }

        public FieldCheckBuilder Field(ResourceMessage message, bool variants = false)
        {
            var builder = new FieldCheckBuilder(_root, message, variants);
            _builders.Add(builder);
            return builder;
        }

        public ConstructorCheckBuilder Constructor(string message = "")
        {
            return Constructor(new ResourceMessage(message));
        }

        public ConstructorCheckBuilder Constructor(ResourceMessage message)
        {
            var builder = new ConstructorCheckBuilder(_root, message);
            _builders.Add(builder);
            return builder;
        }

        public PropertyCheckBuilder Property(string message = "")
        {
            return Property(new ResourceMessage(message));
        }

        public PropertyCheckBuilder Property(ResourceMessage message)
        {
            var builder = new PropertyCheckBuilder(_root, message);
            _builders.Add(builder);
            return builder;
        }

        public abstract ICheckResult Check(Dictionary<IMember, List<ICheckResult>> results, IMemberCheckBuilder check);
    }
}
