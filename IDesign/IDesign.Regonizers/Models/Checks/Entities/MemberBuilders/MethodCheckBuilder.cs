using System.Collections.Generic;
using IDesign.Recognizers.Models.Checks.Members;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities.MemberBuilders
{
    public class MethodCheckBuilder : AbstractMemberCheckBuilder<IMethod, MethodCheckBuilder, MethodCheck>
    {
        private readonly bool _variants;

        public MethodCheckBuilder(ICheckBuilder<IEntity> root, ResourceMessage message, bool variants = false) : base(root, new MethodCheck(), message)
        {
            _variants = variants;
        }

        public override IEnumerable<IMember> GetCheckable(IEntity entity)
        {
            return _variants ? entity.GetAllMethods() : entity.GetMethods();
        }

        protected override MethodCheckBuilder This() => this;

        /// <inheritdoc cref="MethodCheck.ReturnType(SyntaxTree.Abstractions.Entities.IEntity)"/>
        public MethodCheckBuilder ReturnType(IEntity entity)
        {
            Check.ReturnType(entity);
            return This();
        }

        /// <inheritdoc cref="MethodCheck.ReturnType(string)"/>
        public MethodCheckBuilder ReturnType(string entity)
        {
            Check.ReturnType(entity);
            return This();
        }
    }
}
