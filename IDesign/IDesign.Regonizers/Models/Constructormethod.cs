﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Recognizers
{
    public class Constructormethod : IMethod
    {
        public Constructormethod(ConstructorDeclarationSyntax constructor)
        {
            Constructor = constructor;
        }

        public ConstructorDeclarationSyntax Constructor { get; set; }

        public BlockSyntax GetBody()
        {
            return Constructor.Body;
        }

        public SyntaxTokenList GetModifiers()
        {
            return Constructor.Modifiers;
        }

        public string GetName()
        {
            return Constructor.Identifier.ToString();
        }

        public string GetReturnType()
        {
            return "void";
        }

        public string GetSuggestionName()
        {
            return GetName() + "()";
        }

        public SyntaxNode GetSuggestionNode()
        {
            return Constructor;
        }

        public IEnumerable<string> GetParameterTypes()
        {
            return Constructor.ParameterList.Parameters.Select(x => (x.Type is TypeSyntax id ? id.ToString() : "")).ToList();
        }

        public IEnumerable<ParameterSyntax> GetParameters()
        {
            return Constructor.ParameterList.Parameters;
        }

        public IEnumerable<string> GetArguments()
        {
            return Constructor.Initializer.ArgumentList.Arguments.ToList().Select(x => x.ToString());
        }
    }
}