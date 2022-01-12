using System.Collections.Generic;
using IDesign.Recognizers.Models.Checks.Members;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities.MemberBuilders
{
    public class FieldCheckBuilder : AbstractMemberCheckBuilder<IField, FieldCheckBuilder, FieldCheck>
    {
        private readonly bool _variants;

        public FieldCheckBuilder(ICheckBuilder<IEntity> root, ResourceMessage message, bool variants = false) : base(root, new FieldCheck(), message)
        {
            _variants = variants;
        }

        public override IEnumerable<IMember> GetCheckable(IEntity entity)
        {
            return _variants ? entity.GetAllFields() : entity.GetFields();
        }

        protected override FieldCheckBuilder This() => this;

        /// <inheritdoc cref="FieldCheckBuilder.Type(IEntity, bool)"/>
        public FieldCheckBuilder Type(IEntity entity, bool includeGeneric = true)
        {
            Check.Type(entity, includeGeneric);
            return This();
        }

        /// <inheritdoc cref="FieldCheckBuilder.Type(string, bool)"/>
        public FieldCheckBuilder Type(string type, bool includeGeneric = true)
        {
            Check.Type(type, includeGeneric);
            return This();
        }
    }
}
