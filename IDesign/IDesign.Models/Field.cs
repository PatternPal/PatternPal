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

        FieldDeclarationSyntax field { get; set; }
        VariableDeclaratorSyntax variable { get; set; }
        public SyntaxTokenList GetModifiers() => field.Modifiers;
        public string GetName() => variable.Identifier.ToString();

        public TypeSyntax GetFieldType() => field.Declaration.Type;
    }
}
