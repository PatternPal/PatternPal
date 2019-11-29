using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public class PropertyField : IField
    {
        public PropertyField(PropertyDeclarationSyntax property)
        {
            this.Property = property;
        }

        public PropertyDeclarationSyntax Property { get; set; }

        public SyntaxTokenList GetModifiers()
        {
            return Property.Modifiers;
        }

        public string GetName()
        {
            return Property.Identifier.ToString();
        }

        public TypeSyntax GetFieldType()
        {
            return Property.Type;
        }
    }
}