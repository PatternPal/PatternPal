using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Field {
    public class Field : IField {
        private readonly FieldDeclarationSyntax _fieldDeclaration;
        private readonly VariableDeclarationSyntax _variable;

        public Field(FieldDeclarationSyntax field) {
            _fieldDeclaration = field;
            _variable = field.Declaration;
        }

        public string GetName() {
            return string.Join(", ", _variable.Variables.Select(v => v.Identifier.ToString()));
        }

        public TypeSyntax GetFieldType() {
            return _fieldDeclaration.Declaration.Type;
        }

        public IEnumerable<IModifier> GetModifiers() {
            return _fieldDeclaration.Modifiers.ToModifiers();
        }

        public SyntaxNode GetSyntaxNode() {
            return _fieldDeclaration;
        }

        public override string ToString() { return GetName(); }
    }
}
