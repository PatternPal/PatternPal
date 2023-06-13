namespace PatternPal.SyntaxTree.Abstractions.Members
{
    /// <summary>
    /// A member of an <see cref="IEntity"/>, like constructors, fields etc.
    /// </summary>
    public interface IMember : IModified, IChild<IEntity>
    {
        /// <summary>
        /// Returns, if available, the return type of the member.
        /// Else it returns the type of the member.
        /// </summary>
        /// <returns>The <see cref="SyntaxNode"/> of the return type of the member.</returns>
        SyntaxNode GetReturnType();
    }

    // TODO Is unused, can be deleted?
    public static class MemberExtension
    {
        /// <summary>
        ///     For a <b>constructor</b> this will be the <b>class name</b>,<br/>
        ///     For a <b>method</b> this will be the <b>return type</b>, void is null,<br/>
        ///     For a <b>field</b> this will be the <b>field type</b>,<br/>
        ///     For a <b>property</b> this will be the <b>property type</b>,<br/>
        ///     Anything else will be null
        /// </summary>
        /// <returns>The type of this member</returns>
        public static string GetMemberType(this IMember member)
        {
            var s = GetMemberTypeT(member);
            if (s == null) return null;
            return s == "void" ? null : s;
        }

        private static string GetMemberTypeT(IMember member)
        {
            switch (member) {
                case IMethod method: return method.GetReturnType()?.ToString();
                case IConstructor constructor: return constructor.GetConstructorType();
                case IField field: return field.GetFieldType().ToString();
                case IProperty property: return property.GetPropertyType().ToString();
                default: return null;
            }
        }
    }
}
