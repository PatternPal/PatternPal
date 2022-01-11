using IDesign.Recognizers.Models.Checks.Members;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities.MemberBuilders
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
