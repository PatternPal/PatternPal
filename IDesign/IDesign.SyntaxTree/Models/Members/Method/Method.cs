using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Members.Method {
    public class Method : AbstractNode, IMethod {
        private readonly MethodDeclarationSyntax _methodDeclaration;
        private readonly IEntity _parent;

        public Method(MethodDeclarationSyntax node, IEntity parent) : base(node, parent.GetRoot()) {
            _methodDeclaration = node;
            _parent = parent;
        }

        public override string GetName() => _methodDeclaration.Identifier.ToString();
        public IEnumerable<IModifier> GetModifiers() => _methodDeclaration.Modifiers.ToModifiers();
        public IEnumerable<TypeSyntax> GetParameters() => _methodDeclaration.ParameterList.ToParameters();

        public CSharpSyntaxNode GetBody() =>
            (CSharpSyntaxNode)_methodDeclaration.Body ?? _methodDeclaration.ExpressionBody;

        public TypeSyntax GetReturnType() => _methodDeclaration.ReturnType;
        public IEntity GetParent() => _parent;

        public override string ToString() => $"{GetName()}()";
    }
}
