using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Members.Method
{
    public class Method : AbstractNode, IMethod
    {
        private readonly MethodDeclarationSyntax _methodDeclaration;
        private readonly IEntity _parent;
        public List<RelationMethod> RelationMethods = new List<RelationMethod>();
        public Method(MethodDeclarationSyntax node, IEntity parent) : base(node, parent?.GetRoot())
        {
            _methodDeclaration = node;
            _parent = parent;
        }

        public override string GetName()
        {
            return _methodDeclaration.Identifier.ToString();
        }

        public IEnumerable<IModifier> GetModifiers()
        {
            return _methodDeclaration.Modifiers.ToModifiers();
        }

        public IEnumerable<TypeSyntax> GetParameters()
        {
            return _methodDeclaration.ParameterList.ToParameters();
        }

        public CSharpSyntaxNode GetBody()
        {
            return (CSharpSyntaxNode)_methodDeclaration.Body ?? _methodDeclaration.ExpressionBody;
        }

        public TypeSyntax GetReturnType()
        {
            return _methodDeclaration.ReturnType;
        }

        public MethodDeclarationSyntax GetDeclarationSyntax()
        {
            return _methodDeclaration;
        }

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
