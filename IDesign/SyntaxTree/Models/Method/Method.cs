using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Method {
    public class Method : IMethod {
        private readonly MethodDeclarationSyntax _methodDeclaration;

        public Method(MethodDeclarationSyntax method) {
            _methodDeclaration = method;
        }

        public string GetName() {
            return _methodDeclaration.Identifier.ToString();
        }

        public SyntaxNode GetSyntaxNode() {
            return _methodDeclaration;
        }

        public IEnumerable<IModifier> GetModifiers() {
            return _methodDeclaration.Modifiers.ToModifiers();
        }

        public IEnumerable<TypeSyntax> GetParameters() {
            return _methodDeclaration.ParameterList.ToParameters();
        }

        public CSharpSyntaxNode GetBody() {
            return (CSharpSyntaxNode)_methodDeclaration.Body ?? _methodDeclaration.ExpressionBody;
        }

        public TypeSyntax GetReturnType() {
            return _methodDeclaration.ReturnType;
        }

        public override string ToString() {
            return $"${GetName()}()";
        }
    }
}
