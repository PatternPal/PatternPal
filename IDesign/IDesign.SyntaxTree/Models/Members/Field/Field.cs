using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Members.Field {
    public class Field : AbstractNode, IField {
        private readonly FieldDeclarationSyntax _fieldDeclaration;
        private readonly VariableDeclarationSyntax _variable;
        private readonly IEntity _parent;

        public Field(FieldDeclarationSyntax node, IEntity parent) : base(node, parent?.GetRoot()) {
            _fieldDeclaration = node;
            _variable = node.Declaration;
            _parent = parent;
        }

        public override string GetName() {
            return string.Join(", ", _variable.Variables.Select(v => v.Identifier.ToString()));
        }

        public TypeSyntax GetFieldType() => _fieldDeclaration.Declaration.Type;

        public IEnumerable<IModifier> GetModifiers() => _fieldDeclaration.Modifiers.ToModifiers();

        public IEntity GetParent() => _parent;

        public override string ToString() => GetName();
    }
}
