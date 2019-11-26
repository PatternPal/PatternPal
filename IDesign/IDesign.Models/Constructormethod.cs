using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public class Constructormethod : IMethod
    {

        public Constructormethod(ConstructorDeclarationSyntax constructor)
        {
            this.constructor = constructor;
        }


        public ConstructorDeclarationSyntax constructor { get; set; }
        public BlockSyntax GetBody() => constructor.Body;

        public SyntaxTokenList GetModifiers() => constructor.Modifiers;

        public string GetName() => constructor.Identifier.ToString();

        public string GetReturnType()
        {

            return "void";
        }
    }
}
