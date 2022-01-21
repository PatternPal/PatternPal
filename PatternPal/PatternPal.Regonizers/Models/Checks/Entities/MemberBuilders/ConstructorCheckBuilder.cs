using PatternPal.Recognizers.Models.Checks.Members;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Entities.MemberBuilders
{
    public class ConstructorCheckBuilder
        : AbstractMemberCheckBuilder<IConstructor, ConstructorCheckBuilder, ConstructorCheck>
    {
        public ConstructorCheckBuilder(ICheckBuilder<IEntity> root, ResourceMessage message) : base(root, new ConstructorCheck(), message)
        {
        }

        protected override ConstructorCheckBuilder This() => this;
    }
}
