using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;

namespace SyntaxTree.Models.Property {
    public class PropertyField : IField {
        private readonly Property _property;
        
        public PropertyField(Property property) { _property = property; }

        public string GetName() { return _property.GetName(); }

        public SyntaxNode GetSyntaxNode() { return _property.GetSyntaxNode(); }

        public IEnumerable<IModifier> GetModifiers() { return _property.GetModifiers(); }

        public TypeSyntax GetFieldType() { return _property.GetPropertyType(); }

        public override string ToString() { return GetName(); }
    }
}
