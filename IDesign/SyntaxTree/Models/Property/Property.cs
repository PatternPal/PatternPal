using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Property {
    public class Property : IProperty {
        internal readonly PropertyDeclarationSyntax propertyDeclarationSyntax;

        public Property(PropertyDeclarationSyntax propertyDeclarationSyntax) {
            this.propertyDeclarationSyntax = propertyDeclarationSyntax;
        }

        public string GetName() {
            return propertyDeclarationSyntax.Identifier.Text;
        }

        public SyntaxNode GetSyntaxNode() {
            return propertyDeclarationSyntax;
        }

        public IEnumerable<IModifier> GetModifiers() {
            return propertyDeclarationSyntax.Modifiers.ToModifiers();
        }

        public bool HasGetter() {
            if (propertyDeclarationSyntax.ExpressionBody != null) return true;
            if (propertyDeclarationSyntax.AccessorList == null) return false;
            return propertyDeclarationSyntax.AccessorList.Accessors.Any(SyntaxKind.GetAccessorDeclaration);
        }

        public bool HasSetter() {
            if (propertyDeclarationSyntax.AccessorList == null) return false;
            return propertyDeclarationSyntax.AccessorList.Accessors.Any(SyntaxKind.SetAccessorDeclaration);
        }

        public IMethod GetGetter() {
            return new PropertyGetMethod(
                this, propertyDeclarationSyntax.AccessorList?
                    .Accessors.First(s => s.Kind() == SyntaxKind.GetAccessorDeclaration)
            );
        }

        public IMethod GetSetter() {
            return new PropertySetMethod(
                this, propertyDeclarationSyntax.AccessorList?
                    .Accessors.First(s => s.Kind() == SyntaxKind.SetAccessorDeclaration)
            );
        }

        public bool IsField() {
            if (propertyDeclarationSyntax.AccessorList == null) return false;
            var accessors = propertyDeclarationSyntax.AccessorList.Accessors;
            return accessors.Any(SyntaxKind.GetAccessorDeclaration) &&
                   accessors.All(a => a.Body == null && a.ExpressionBody == null);
        }

        public IField GetField() { return new PropertyField(this); }
        
        public TypeSyntax GetPropertyType() { return propertyDeclarationSyntax.Type; }
    }
}
