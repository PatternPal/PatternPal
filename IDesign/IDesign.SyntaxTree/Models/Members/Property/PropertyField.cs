using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Models.Members.Property
{
    public class PropertyField : IField
    {
        private readonly Property _property;

        public PropertyField(Property property) { _property = property; }

        public SyntaxNode GetSyntaxNode()
        {
            return _property.GetSyntaxNode();
        }

        public IRoot GetRoot()
        {
            return _property.GetRoot();
        }

        public IEntity GetParent()
        {
            return _property.GetParent();
        }

        public IEnumerable<IModifier> GetModifiers()
        {
            return _property.GetModifiers();
        }

        public TypeSyntax GetFieldType()
        {
            return _property.GetPropertyType();
        }

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
