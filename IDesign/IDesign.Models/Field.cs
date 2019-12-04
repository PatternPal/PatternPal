using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public class Field : IField
    {
        public Field(FieldDeclarationSyntax field, VariableDeclaratorSyntax variable)
        {
            FieldDeclaration = field;
            Variable = variable;
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

        public string GetSuggestionName()
        {
            return GetName();
        }

        public SyntaxNode GetSuggestionNode()
        {
            return FieldDeclaration;
        }
    }
}