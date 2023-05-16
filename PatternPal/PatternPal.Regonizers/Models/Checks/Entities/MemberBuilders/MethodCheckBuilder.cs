using System.Collections.Generic;
using PatternPal.Recognizers.Models.Checks.Members;
using PatternPal.Recognizers.Models.Output;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Entities.MemberBuilders
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
    }
}
