using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using IDesign.Checks;

namespace IDesign.Recognizers
{
    public class Field : IField
    {
        public Field(FieldDeclarationSyntax field, VariableDeclaratorSyntax variable)
        {
            this.FieldDeclaration = field;
            this.Variable = variable;
        }

        private FieldDeclarationSyntax FieldDeclaration { get; }
        private VariableDeclaratorSyntax Variable { get; }

        public SyntaxTokenList GetModifiers()
        {
            return FieldDeclaration.Modifiers;
        }

        public string GetName()
        {
            return Variable.Identifier.ToString();
        }

        public TypeSyntax GetFieldType()
        {
            return FieldDeclaration.Declaration.Type;
        }
    }
}