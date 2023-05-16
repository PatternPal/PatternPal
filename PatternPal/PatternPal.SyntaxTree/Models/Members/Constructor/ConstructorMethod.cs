using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;

namespace PatternPal.SyntaxTree.Models.Members.Constructor
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

        public IEntity GetParent()
        {
            return constructor.GetParent();
        }

        public SyntaxNode GetReturnType()
        {
            return GetParent().GetSyntaxNode();
        }

        public override string ToString()
        {
            return constructor.ToString();
        }
    }
}
