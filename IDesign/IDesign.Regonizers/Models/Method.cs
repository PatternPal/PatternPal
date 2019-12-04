using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.Recognizers
{
    public class Method : IMethod
    {
        public Method(MethodDeclarationSyntax method)
        {
            MethodDeclaration = method;
        }


        public MethodDeclarationSyntax MethodDeclaration { get; set; }

        public BlockSyntax GetBody()
        {
            return MethodDeclaration.Body;
        }

        public SyntaxTokenList GetModifiers()
        {
            return MethodDeclaration.Modifiers;
        }

        public string GetName()
        {
            return MethodDeclaration.Identifier.ToString();
        }

        public string GetReturnType()
        {
            return MethodDeclaration.ReturnType.ToString();
        }

        public string GetSuggestionName()
        {
            return GetName() + "()";
        }

        public SyntaxNode GetSuggestionNode()
        {
            return MethodDeclaration;
        }

        public List<string> GetParameters()
        {
            return MethodDeclaration.ParameterList.Parameters.Select(x => (x.Type is TypeSyntax id ? id.ToString() : "")).ToList();
        }
    }
}