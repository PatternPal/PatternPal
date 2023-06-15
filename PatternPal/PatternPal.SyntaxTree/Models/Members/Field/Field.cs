#region

using System.Linq;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Utils;

#endregion

namespace PatternPal.SyntaxTree.Models.Members.Field
{
    /// <inheritdoc cref="IField"/>
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

        /// <inheritdoc />
        public override string GetName()
        {
            return string.Join(", ", _variable.Variables.Select(v => v.Identifier.ToString()));
        }

        /// <inheritdoc />
        public TypeSyntax GetFieldType()
        {
            return _fieldDeclaration.Declaration.Type;
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return _fieldDeclaration.Modifiers.ToModifiers();
        }

        /// <inheritdoc />
        public IEntity GetParent()
        {
            return _parent;
        }

        /// <inheritdoc />
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
