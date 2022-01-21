using PatternPal.Recognizers.Models.Checks.Members;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Entities.MemberBuilders
{
    public class PropertyCheckBuilder : AbstractMemberCheckBuilder<IProperty, PropertyCheckBuilder, PropertyCheck>
    {
        public PropertyCheckBuilder(ICheckBuilder<IEntity> root, ResourceMessage message) : base(
            root, new PropertyCheck(), message
        )
        {
        }

        protected override PropertyCheckBuilder This() => this;
    }
}
