using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Models.Output;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;

namespace PatternPal.Recognizers.Models.Checks.Members
{
    public abstract class AbstractMemberListCheck<T, R> : AbstractListCheck<T, R>
        where T : IMember where R : AbstractMemberListCheck<T, R>
    {
        /// <summary>
        ///     Checks if the current member has all the modifiers
        ///     Use `Modifiers.Private.Not()` for when you want a modifier to be not there
        /// </summary>
        /// <param name="modifiers">The modifiers to match</param>
        /// <returns>The original check</returns>
        public R Modifiers(params IModifier[] modifiers)
        {
            _checks.Add((ICheck<T>)new ModifierCheck(modifiers));
            return This();
        }

        /// <summary>
        ///     Checks if the type of the member is of type entity
        /// </summary>
        /// <param name="entity">The entity to check</param>
        /// <returns>The original Check, for fluent programming</returns>
        /// <seealso cref="MemberExtension.GetMemberType"/>
        public R Type(IEntity entity)
        {
            return Type(entity.GetName());
        }

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
