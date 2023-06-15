#region

using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Utils;

#endregion

namespace PatternPal.SyntaxTree.Models.Members.Property
{
    /// <inheritdoc cref="IProperty"/>
    public class Property : AbstractNode, IProperty
    {
        private readonly IEntity _parent;
        internal readonly PropertyDeclarationSyntax PropertyDeclarationSyntax;

        public Property(PropertyDeclarationSyntax node, IEntity parent) : base(node, parent?.GetRoot())
        {
            PropertyDeclarationSyntax = node;
            _parent = parent;
        }

        /// <inheritdoc />
        public override string GetName()
        {
            return PropertyDeclarationSyntax.Identifier.Text;
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return PropertyDeclarationSyntax.Modifiers.ToModifiers();
        }

        /// <inheritdoc />
        public IField GetField()
        {
            return new PropertyField(this);
        }

        /// <inheritdoc />
        public TypeSyntax GetPropertyType()
        {
            return PropertyDeclarationSyntax.Type;
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
            if (PropertyDeclarationSyntax.ExpressionBody != null)
            {
                return true;
            }

            if (PropertyDeclarationSyntax.AccessorList == null)
            {
                return false;
            }

            return PropertyDeclarationSyntax.AccessorList.Accessors.Any(SyntaxKind.GetAccessorDeclaration);
        }

        /// <inheritdoc />
        public bool HasSetter()
        {
            if (PropertyDeclarationSyntax.AccessorList == null)
            {
                return false;
            }

            return PropertyDeclarationSyntax.AccessorList.Accessors.Any(SyntaxKind.SetAccessorDeclaration);
        }

        /// <inheritdoc />
        public IMethod GetGetter()
        {
            return new PropertyGetMethod(
                this, PropertyDeclarationSyntax.AccessorList?
                    .Accessors.First(s => s.Kind() == SyntaxKind.GetAccessorDeclaration)
            );
        }

        /// <inheritdoc />
        public IMethod GetSetter()
        {
            return new PropertySetMethod(
                this, PropertyDeclarationSyntax.AccessorList?
                    .Accessors.First(s => s.Kind() == SyntaxKind.SetAccessorDeclaration)
            );
        }

        /// <inheritdoc />
        public bool IsField()
        {
            if (PropertyDeclarationSyntax.AccessorList == null)
            {
                return false;
            }

            SyntaxList<AccessorDeclarationSyntax> accessors = PropertyDeclarationSyntax.AccessorList.Accessors;
            return accessors.Any(SyntaxKind.GetAccessorDeclaration) &&
                   accessors.All(a => a.Body == null && a.ExpressionBody == null);
        }
    }
}
