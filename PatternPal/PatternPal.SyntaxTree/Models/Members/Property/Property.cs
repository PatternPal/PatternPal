using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Members.Property
{
    /// <inheritdoc cref="IProperty"/>
    public class Property : AbstractNode, IProperty
    {
        private readonly IEntity _parent;
        internal readonly PropertyDeclarationSyntax propertyDeclarationSyntax;

        public Property(PropertyDeclarationSyntax node, IEntity parent) : base(node, parent?.GetRoot())
        {
            propertyDeclarationSyntax = node;
            _parent = parent;
        }

        /// <inheritdoc />
        public override string GetName()
        {
            return propertyDeclarationSyntax.Identifier.Text;
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return propertyDeclarationSyntax.Modifiers.ToModifiers();
        }

        /// <inheritdoc />
        public IField GetField()
        {
            return new PropertyField(this);
        }

        /// <inheritdoc />
        public TypeSyntax GetPropertyType()
        {
            return propertyDeclarationSyntax.Type;
        }

        /// <inheritdoc />
        public IEntity GetParent()
        {
            return _parent;
        }

        /// <inheritdoc />
        public SyntaxNode GetReturnType()
        {
            return GetPropertyType();
        }

        /// <inheritdoc />
        public bool HasGetter()
        {
            if (propertyDeclarationSyntax.ExpressionBody != null)
            {
                return true;
            }

            if (propertyDeclarationSyntax.AccessorList == null)
            {
                return false;
            }

            return propertyDeclarationSyntax.AccessorList.Accessors.Any(SyntaxKind.GetAccessorDeclaration);
        }

        /// <inheritdoc />
        public bool HasSetter()
        {
            if (propertyDeclarationSyntax.AccessorList == null)
            {
                return false;
            }

            return propertyDeclarationSyntax.AccessorList.Accessors.Any(SyntaxKind.SetAccessorDeclaration);
        }

        /// <inheritdoc />
        public IMethod GetGetter()
        {
            return new PropertyGetMethod(
                this, propertyDeclarationSyntax.AccessorList?
                    .Accessors.First(s => s.Kind() == SyntaxKind.GetAccessorDeclaration)
            );
        }

        /// <inheritdoc />
        public IMethod GetSetter()
        {
            return new PropertySetMethod(
                this, propertyDeclarationSyntax.AccessorList?
                    .Accessors.First(s => s.Kind() == SyntaxKind.SetAccessorDeclaration)
            );
        }

        /// <inheritdoc />
        public bool IsField()
        {
            if (propertyDeclarationSyntax.AccessorList == null)
            {
                return false;
            }

            var accessors = propertyDeclarationSyntax.AccessorList.Accessors;
            return accessors.Any(SyntaxKind.GetAccessorDeclaration) &&
                   accessors.All(a => a.Body == null && a.ExpressionBody == null);
        }
    }
}
