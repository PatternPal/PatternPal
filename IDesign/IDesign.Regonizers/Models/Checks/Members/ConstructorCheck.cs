using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Members
{
    public class ConstructorCheck : AbstractMemberListCheck<IConstructor, ConstructorCheck>
    {
        protected override ConstructorCheck This() => this;
    }
}
