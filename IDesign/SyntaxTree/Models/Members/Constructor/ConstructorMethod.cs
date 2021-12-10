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
        public readonly IConstructor constructor;
        
        public ConstructorMethod(IConstructor constructor) { this.constructor = constructor; }
        
        public string GetName() => constructor.GetName();

        public SyntaxNode GetSyntaxNode() => constructor.GetSyntaxNode();
        public IRoot GetRoot() => constructor.GetRoot();

        public IEnumerable<IModifier> GetModifiers() => constructor.GetModifiers();

        public IEnumerable<TypeSyntax> GetParameters() => constructor.GetParameters();

        public CSharpSyntaxNode GetBody() => constructor.GetBody();

        public TypeSyntax GetReturnType() => null;
        public IEntity GetParent() => constructor.GetParent();
    }
}
