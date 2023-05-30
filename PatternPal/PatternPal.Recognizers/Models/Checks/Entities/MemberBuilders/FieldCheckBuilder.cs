using System.Collections.Generic;
using PatternPal.Recognizers.Models.Checks.Members;
using PatternPal.Recognizers.Models.Output;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Entities.MemberBuilders
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
    }
}
