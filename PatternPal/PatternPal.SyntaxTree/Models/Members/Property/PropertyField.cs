using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;

namespace PatternPal.SyntaxTree.Models.Members.Property
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

        public SyntaxNode GetReturnType()
        {
            return _property.GetReturnType();
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
