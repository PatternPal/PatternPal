using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Members
{
    public class ConstructorCheck : AbstractMemberListCheck<IConstructor, ConstructorCheck>
    {
        protected override ConstructorCheck This() => this;
    }
}
