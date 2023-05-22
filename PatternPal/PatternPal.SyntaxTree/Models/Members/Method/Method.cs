using Microsoft.CodeAnalysis.CSharp;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Members.Method
{
    /// <inheritdoc cref="IMethod"/>
    public class Method : AbstractNode, IMethod
    {
        private readonly MethodDeclarationSyntax _methodDeclaration;
        private readonly IEntity _parent;
        public Method(MethodDeclarationSyntax node, IEntity parent) : base(node, parent?.GetRoot())
        {
            _methodDeclaration = node;
            _parent = parent;
        }

        /// <inheritdoc />
        public override string GetName()
        {
            return _methodDeclaration.Identifier.ToString();
        }

        /// <inheritdoc />
        public IEnumerable<IModifier> GetModifiers()
        {
            return _methodDeclaration.Modifiers.ToModifiers();
        }

        /// <inheritdoc />
        public IEnumerable<TypeSyntax> GetParameters()
        {
            return _methodDeclaration.ParameterList.ToParameters();
        }

        /// <inheritdoc />
        public CSharpSyntaxNode GetBody()
        {
            return (CSharpSyntaxNode)_methodDeclaration.Body ?? _methodDeclaration.ExpressionBody;
        }

        /// <inheritdoc />
        public SyntaxNode GetReturnType()
        {
            return _methodDeclaration.ReturnType;
        }

        public MethodDeclarationSyntax GetDeclarationSyntax()
        {
            return _methodDeclaration;
        }

        /// <inheritdoc />
        public IEntity GetParent()
        {
            return _parent;
        }

        public override string ToString()
        {
            return $"{GetName()}()";
        }
    }
}
