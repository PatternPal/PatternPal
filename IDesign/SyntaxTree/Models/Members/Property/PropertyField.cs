using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Models.Members.Property {
    public class PropertyField : IField {
        private readonly Property _property;

        public PropertyField(Property property) { _property = property; }

        public SyntaxNode GetSyntaxNode() => _property.GetSyntaxNode();
        public IRoot GetRoot() => _property.GetRoot();
        public IEntity GetParent() => _property.GetParent();

        public IEnumerable<IModifier> GetModifiers() => _property.GetModifiers();
        public TypeSyntax GetFieldType() => _property.GetPropertyType();

        public string GetName() => _property.GetName();
        public override string ToString() => GetName();
    }
}
