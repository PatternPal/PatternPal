using System;
using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models.Checks.Entities.GroupChecks;
using IDesign.Recognizers.Models.Checks.Members;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities
{
    public interface IMemberCheckBuilder : ICheckBuilder<IEntity>
    {
        IEnumerable<IMember> GetCheckable(IEntity entity);

        IEnumerable<ICheck<IMember>> Checks();

        ResourceMessage GetMessage();
    }

    public abstract partial class AbstractMemberCheckBuilder<T, R, C> : IMemberCheckBuilder
        where R : ICheckBuilder<IEntity>
        where T : IMember
        where C : AbstractMemberListCheck<T, C>
    {
        protected readonly ICheckBuilder<IEntity> Root;
        protected readonly C Check;
        protected readonly ResourceMessage Message;

        protected AbstractMemberCheckBuilder(ICheckBuilder<IEntity> root, C check, ResourceMessage message)
        {
            Root = root;
            Check = check;
            Message = message;
        }

        public AnyMemberGroupCheck Any => Root.Any;
        public AllMemberGroupCheck All => Root.All;
        public EntityCheck Build() => Root.Build();

        public virtual IEnumerable<IMember> GetCheckable(IEntity entity) => entity.GetMembers().OfType<T>().Cast<IMember>();
        public IEnumerable<ICheck<IMember>> Checks() => Check.Select(Convert);
        public ResourceMessage GetMessage() => Message;

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
            if (element == null) return _from.Check(default);
            if (element is F node) return _from.Check(node);
            throw new InvalidCastException();
        }
    }
}
