using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Members
{
    // TODO QA: XML-comment
    public class MethodCheck : AbstractMemberListCheck<IMethod, MethodCheck>
    {
        protected override MethodCheck This() => this;
    }
}
