using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Members;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Models.Members.Constructor
{
    public class ConstructorMethod : IMethod
    {
        public readonly IConstructor constructor;

        public ConstructorMethod(IConstructor constructor) { this.constructor = constructor; }

        public string GetName()
        {
            return constructor.GetName();
        }

        public SyntaxNode GetSyntaxNode()
        {
            return constructor.GetSyntaxNode();
        }

        public IRoot GetRoot()
        {
            return constructor.GetRoot();
        }

        public IEnumerable<IModifier> GetModifiers()
        {
            return constructor.GetModifiers();
        }

        public IEnumerable<TypeSyntax> GetParameters()
        {
            return constructor.GetParameters();
        }

        public CSharpSyntaxNode GetBody()
        {
            return constructor.GetBody();
        }

        public TypeSyntax GetReturnType()
        {
            return null;
        }

        public IEntity GetParent()
        {
            return constructor.GetParent();
        }

        public override string ToString()
        {
            return constructor.ToString();
        }
    }
}
