using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Members
{
    public class PropertyCheck : AbstractMemberListCheck<IProperty, PropertyCheck>
    {
        protected override PropertyCheck This() => this;
    }
}
