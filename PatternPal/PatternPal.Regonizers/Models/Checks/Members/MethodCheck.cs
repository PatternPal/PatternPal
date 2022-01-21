using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Members
{
    public class MethodCheck : AbstractMemberListCheck<IMethod, MethodCheck>
    {
        /// <summary>
        ///     Exactly the same as <see cref="AbstractMemberListCheck{T,R}.Type(SyntaxTree.Abstractions.Entities.IEntity)"/>
        /// </summary>
        public MethodCheck ReturnType(IEntity entity)
        {
            return Type(entity);
        }

        /// <summary>
        ///     Exactly the same as <see cref="AbstractMemberListCheck{T,R}.Type(string)"/>
        /// </summary>
        public MethodCheck ReturnType(string entity)
        {
            return Type(entity);
        }

        protected override MethodCheck This() => this;
    }
}
