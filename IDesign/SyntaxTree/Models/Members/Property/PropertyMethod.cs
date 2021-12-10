using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Models.Members.Property {
    public abstract class PropertyMethod : IMethod {
        protected readonly Property property;
        private readonly AccessorDeclarationSyntax _accessor;

        protected PropertyMethod(Property property, AccessorDeclarationSyntax accessor) {
            this.property = property;
            _accessor = accessor;
        }

        public abstract string GetName();
        public abstract IEnumerable<TypeSyntax> GetParameters();
        public abstract TypeSyntax GetReturnType();

        public SyntaxNode GetSyntaxNode() => _accessor ?? property.GetSyntaxNode();
        public IRoot GetRoot() => property.GetRoot();
        public IEnumerable<IModifier> GetModifiers() => property.GetModifiers();

        public CSharpSyntaxNode GetBody() {
            return (CSharpSyntaxNode)_accessor?.Body
                   ?? _accessor?.ExpressionBody
                   ?? property.propertyDeclarationSyntax.ExpressionBody;
        }

        public IEntity GetParent() => property.GetParent();

        public override string ToString() { return GetName(); }
    }

    public class PropertyGetMethod : PropertyMethod {
        public PropertyGetMethod(Property property, AccessorDeclarationSyntax accessor) : base(property, accessor) {
        }

        public override string GetName() { return $"${property.GetName()}_get"; }

        public override IEnumerable<TypeSyntax> GetParameters() { return Array.Empty<TypeSyntax>(); }
        public override TypeSyntax GetReturnType() { return property.GetPropertyType(); }
    }

    public class PropertySetMethod : PropertyMethod {
        public PropertySetMethod(Property property, AccessorDeclarationSyntax accessor) : base(property, accessor) {
        }

        public override string GetName() { return $"${property.GetName()}_set"; }

        public override IEnumerable<TypeSyntax> GetParameters() { return new[] { property.GetPropertyType() }; }
        public override TypeSyntax GetReturnType() { return null; }
    }
}
