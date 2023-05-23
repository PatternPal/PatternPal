using PatternPal.SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Members
{
    public class MethodCheck : AbstractMemberListCheck<IMethod, MethodCheck>
    {
        protected override MethodCheck This() => this;
    }
}
