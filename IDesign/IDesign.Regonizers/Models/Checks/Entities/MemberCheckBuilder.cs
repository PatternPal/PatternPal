using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Checks.Members;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities
{
    public interface IMemberCheckBuilder : ICheckBuilder<IEntity>
    {
        IEnumerable<IMember> GetCheckable(IEntity entity);

        IEnumerable<ICheck<IMember>> Checks();
    }

    public abstract partial class AbstractMemberCheckBuilder<T, R, C> : IMemberCheckBuilder
        where R : ICheckBuilder<IEntity>
        where T : IMember
        where C : AbstractMemberListCheck<T, C>
    {
        protected readonly ICheckBuilder<IEntity> Root;
        protected readonly C Check;

        protected AbstractMemberCheckBuilder(ICheckBuilder<IEntity> root, C check)
        {
            Root = root;
            Check = check;
        }

        public AnyMemberGroupCheck Any => Root.Any;
        public AllMemberGroupCheck All => Root.All;
        public EntityCheck Build() => Root.Build();

        public virtual IEnumerable<IMember> GetCheckable(IEntity entity) => entity.GetMembers().OfType<T>().Cast<IMember>();
        public IEnumerable<ICheck<IMember>> Checks() => Check.Select(Convert);

        private static ICheck<IMember> Convert(ICheck<T> check) => new CheckWrapper<T, IMember>(check);

        protected abstract R This();
    }

    internal class CheckWrapper<F, T> : ICheck<T>
        where T : INode
        where F : T, INode
    {
        private readonly ICheck<F> _from;
        public CheckWrapper(ICheck<F> from) { _from = from; }

        public ICheckResult Check(T element)
        {
            if (element is F node) return _from.Check(node);
            return null;
        }
    }
}
