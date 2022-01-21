using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Members
{
    public class PropertyCheck : AbstractMemberListCheck<IProperty, PropertyCheck>
    {
        protected override PropertyCheck This() => this;
    }
}
