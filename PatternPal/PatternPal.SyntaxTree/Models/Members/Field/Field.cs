using System.Linq;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Members.Field
{
    public class Field : AbstractNode, IField
    {
        private readonly FieldDeclarationSyntax _fieldDeclaration;
        private readonly IEntity _parent;
        private readonly VariableDeclarationSyntax _variable;

        public Field(FieldDeclarationSyntax node, IEntity parent) : base(node, parent?.GetRoot())
        {
            _fieldDeclaration = node;
            _variable = node.Declaration;
            _parent = parent;
        }

        public override string GetName()
        {
            return string.Join(", ", _variable.Variables.Select(v => v.Identifier.ToString()));
        }

        public TypeSyntax GetFieldType()
        {
            return _fieldDeclaration.Declaration.Type;
        }

        public IEnumerable<IModifier> GetModifiers()
        {
            return _fieldDeclaration.Modifiers.ToModifiers();
        }

        public IEntity GetParent()
        {
            return _parent;
        }

        public SyntaxNode GetReturnType()
        {
            return GetFieldType();
        }

        public override string ToString()
        {
            return GetName();
        }
    }
}
