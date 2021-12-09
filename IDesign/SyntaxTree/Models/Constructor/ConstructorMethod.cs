using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;

namespace SyntaxTree.Models.Constructor {
    public class ConstructorMethod: IMethod {
        private readonly Constructor _constructor;
        
        public ConstructorMethod(Constructor constructor) { _constructor = constructor; }
        
        public string GetName() { return _constructor.GetName(); }

        public SyntaxNode GetSyntaxNode() { return _constructor.GetSyntaxNode(); }

        public IEnumerable<IModifier> GetModifiers() { return _constructor.GetModifiers(); }

        public IEnumerable<TypeSyntax> GetParameters() { return _constructor.GetParameters(); }

        public CSharpSyntaxNode GetBody() { return _constructor.GetBody(); }

        public TypeSyntax GetReturnType() { return null; }
    }
}
