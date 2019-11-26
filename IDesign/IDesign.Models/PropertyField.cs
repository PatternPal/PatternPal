using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public class PropertyField : IField
    {
        public PropertyField(PropertyDeclarationSyntax property)
        {
            this.property = property;
        }

        public PropertyDeclarationSyntax property { get; set; }

        public SyntaxTokenList GetModifiers()
        {
            return property.Modifiers;
        }

        public string GetName()
        {
            return property.Identifier.ToString();
        }

        public TypeSyntax GetFieldType()
        {
            return property.Type;
        }
    }
}