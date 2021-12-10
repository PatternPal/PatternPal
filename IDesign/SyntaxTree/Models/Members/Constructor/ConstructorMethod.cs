using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Models.Members.Constructor {
    public class ConstructorMethod: IMethod {
        private readonly Constructor _constructor;
        
        public ConstructorMethod(Constructor constructor) { _constructor = constructor; }
        
        public string GetName() => _constructor.GetName();

        public SyntaxNode GetSyntaxNode() => _constructor.GetSyntaxNode();
        public IRoot GetRoot() => _constructor.GetRoot();

        public IEnumerable<IModifier> GetModifiers() => _constructor.GetModifiers();

        public IEnumerable<TypeSyntax> GetParameters() => _constructor.GetParameters();

        public CSharpSyntaxNode GetBody() => _constructor.GetBody();

        public TypeSyntax GetReturnType() => null;
        public IEntity GetParent() => _constructor.GetParent();
    }
}
