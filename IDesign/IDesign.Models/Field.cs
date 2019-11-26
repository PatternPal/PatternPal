using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public class Field : IField
    {
        public Field(FieldDeclarationSyntax field, VariableDeclaratorSyntax variable)
        {
            this.field = field;
            this.variable = variable;
        }

        private FieldDeclarationSyntax field { get; }
        private VariableDeclaratorSyntax variable { get; }

        public SyntaxTokenList GetModifiers()
        {
            return field.Modifiers;
        }

        public string GetName()
        {
            return variable.Identifier.ToString();
        }

        public TypeSyntax GetFieldType()
        {
            return field.Declaration.Type;
        }
    }
}