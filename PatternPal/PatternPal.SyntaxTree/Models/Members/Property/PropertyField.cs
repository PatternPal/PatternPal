#region

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;

#endregion

namespace PatternPal.SyntaxTree.Models.Members.Property
{
    /// <summary>
    /// An <see cref="IProperty"/> wrapped as an <see cref="IField"/>.
    /// </summary>
    public class PropertyField : IField
    {
        // The property wrapped.
        private readonly Property _property;

        public PropertyField(Property property) { _property = property; }

        /// <inheritdoc />
        public SyntaxNode GetSyntaxNode()
        {
            return _property.GetSyntaxNode();
        }

        /// <inheritdoc />
        public IRoot GetRoot()
        {
            return _property.GetRoot();
        }

        /// <inheritdoc />
        public IEntity GetParent()
        {
            return _property.GetParent();
        }

        /// <inheritdoc />
        public SyntaxNode GetReturnType()
        {
            return _property.GetReturnType();
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return _property.GetModifiers();
        }

        /// <inheritdoc />
        public TypeSyntax GetFieldType()
        {
            return _property.GetPropertyType();
        }

        /// <inheritdoc />
        public string GetName()
        {
            return _property.GetName();
        }

        public override string ToString()
        {
            return GetName();
        }
    }
}
