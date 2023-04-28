using PatternPal.Recognizers.Models.Output;

using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Members
{
    public abstract class AbstractMemberListCheck<T, R> : AbstractListCheck<T, R>
        where T : IMember where R : AbstractMemberListCheck<T, R>
    {
        /// <summary>
        ///     Checks if the type of the member is of type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>The original Check, for fluent programming</returns>
        /// <seealso cref="MemberExtension.GetMemberType"/>
        public R Type(string type)
        {
            return Custom(
                m => m.GetMemberType()?.Equals(type) ?? type == null,
                new ResourceMessage("Type", "Node", type)
            );
        }
    }
}
