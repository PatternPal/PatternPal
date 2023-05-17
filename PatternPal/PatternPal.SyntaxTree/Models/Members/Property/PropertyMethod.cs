using System;
using Microsoft.CodeAnalysis.CSharp;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;

namespace PatternPal.SyntaxTree.Models.Members.Property
{
    public abstract class PropertyMethod : IMethod
    {
        private readonly AccessorDeclarationSyntax _accessor;
        protected readonly Property property;

        protected PropertyMethod(Property property, AccessorDeclarationSyntax accessor)
        {
            this.property = property;
            _accessor = accessor;
        }

        public abstract string GetName();
        public abstract IEnumerable<TypeSyntax> GetParameters();
        public abstract SyntaxNode GetReturnType();

        public SyntaxNode GetSyntaxNode()
        {
            return _accessor ?? property.GetSyntaxNode();
        }

        public IRoot GetRoot()
        {
            return property.GetRoot();
        }

        public IEnumerable<IModifier> GetModifiers()
        {
            return property.GetModifiers();
        }

        public CSharpSyntaxNode GetBody()
        {
            return (CSharpSyntaxNode)_accessor?.Body
                   ?? _accessor?.ExpressionBody
                   ?? property.propertyDeclarationSyntax.ExpressionBody;
        }

        public IEntity GetParent()
        {
            return property.GetParent();
        }

        public override string ToString()
        {
            return GetName();
        }
    }

    public class PropertyGetMethod : PropertyMethod
    {
        public PropertyGetMethod(Property property, AccessorDeclarationSyntax accessor) : base(property, accessor)
        {
        }

        public override string GetName() { return $"{property.GetName()}_get"; }

        public override IEnumerable<TypeSyntax> GetParameters() { return Array.Empty<TypeSyntax>(); }
        public override TypeSyntax GetReturnType() { return property.GetPropertyType(); }
    }

    public class PropertySetMethod : PropertyMethod
    {
        public PropertySetMethod(Property property, AccessorDeclarationSyntax accessor) : base(property, accessor)
        {
        }

        public override string GetName() { return $"{property.GetName()}_set"; }

        public override IEnumerable<TypeSyntax> GetParameters() { return new[] {property.GetPropertyType()}; }
        public override TypeSyntax GetReturnType() { return null; }
    }
}
