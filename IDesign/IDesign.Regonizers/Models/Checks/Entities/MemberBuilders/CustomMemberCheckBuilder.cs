using IDesign.Recognizers.Models.Checks.Members;
using IDesign.Recognizers.Models.Output;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace IDesign.Recognizers.Models.Checks.Entities.MemberBuilders
{
    public class CustomMemberCheckBuilder<N, T> : AbstractMemberCheckBuilder<N, CustomMemberCheckBuilder<N, T>, T>
        where N : IMember
        where T : AbstractMemberListCheck<N, T>
    {
        public CustomMemberCheckBuilder(ICheckBuilder<IEntity> root, T check, ResourceMessage message) : base(
            root, check, message
        )
        {
        }

        protected override CustomMemberCheckBuilder<N, T> This() => this;
    }
}
