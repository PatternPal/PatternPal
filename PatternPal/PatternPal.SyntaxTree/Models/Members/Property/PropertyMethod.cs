using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;

namespace PatternPal.SyntaxTree.Models.Members.Property
{
    /// <summary>
    /// An <see cref="IProperty"/> wrapped as an <see cref="IMethod"/>.
    /// </summary>
    public abstract class PropertyMethod : IMethod
    {
        private readonly AccessorDeclarationSyntax _accessor;
        // The property wrapped.
        protected readonly Property property;

        protected PropertyMethod(Property property, AccessorDeclarationSyntax accessor)
        {
            this.property = property;
            _accessor = accessor;
        }

        /// <inheritdoc />
        public abstract string GetName();

        /// <inheritdoc />
        public abstract IEnumerable<TypeSyntax> GetParameters();

        /// <inheritdoc />
        public abstract SyntaxNode GetReturnType();

        /// <inheritdoc />
        public SyntaxNode GetSyntaxNode()
        {
            return _accessor ?? property.GetSyntaxNode();
        }

        /// <inheritdoc />
        public IRoot GetRoot()
        {
            return property.GetRoot();
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return property.GetModifiers();
        }

        /// <inheritdoc />
        public CSharpSyntaxNode GetBody()
        {
            return (CSharpSyntaxNode)_accessor?.Body
                   ?? _accessor?.ExpressionBody
                   ?? property.propertyDeclarationSyntax.ExpressionBody;
        }

        /// <inheritdoc />
        public IEntity GetParent()
        {
            return property.GetParent();
        }

        public override string ToString()
        {
            return GetName();
        }
    }

    /// <summary>
    /// The getter part of an <see cref="IProperty"/> wrapped as an <see cref="IMethod"/>.
    /// </summary>
    public class PropertyGetMethod : PropertyMethod
    {
        public PropertyGetMethod(Property property, AccessorDeclarationSyntax accessor) : base(property, accessor)
        {
        }

        /// <inheritdoc />
        public override string GetName() { return $"{property.GetName()}_get"; }

        /// <inheritdoc />
        public override IEnumerable<TypeSyntax> GetParameters() { return Array.Empty<TypeSyntax>(); }

        /// <inheritdoc />
        public override TypeSyntax GetReturnType() { return property.GetPropertyType(); }
    }

    /// <summary>
    /// The setter part of an <see cref="IProperty"/> wrapped as an <see cref="IMethod"/>.
    /// </summary>
    public class PropertySetMethod : PropertyMethod
    {
        public PropertySetMethod(Property property, AccessorDeclarationSyntax accessor) : base(property, accessor)
        {
        }

        /// <inheritdoc />
        public override string GetName() { return $"{property.GetName()}_set"; }

        /// <inheritdoc />
        public override IEnumerable<TypeSyntax> GetParameters() { return new[] {property.GetPropertyType()}; }

        /// <inheritdoc />
        public override TypeSyntax GetReturnType() { return null; }
    }
}
