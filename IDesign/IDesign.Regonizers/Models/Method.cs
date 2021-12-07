using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Models
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

        public IEnumerable<string> GetParameterTypes()
        {
            return MethodDeclaration.ParameterList.Parameters
                .Select(x => x.Type is TypeSyntax id ? id.ToString() : "")
                .ToList();
        }

        public string GetReturnType()
        {
            return MethodDeclaration.ReturnType.ToString();
        }

        public ParameterListSyntax GetParameter()
        {
            return MethodDeclaration.ParameterList;
        }

        public string GetSuggestionName()
        {
            return $"{GetName()}()";
        }

        public SyntaxNode GetSuggestionNode()
        {
            return MethodDeclaration;
        }

        public IEnumerable<ParameterSyntax> GetParameters()
        {
            return MethodDeclaration.ParameterList.Parameters;
        }

        public IEnumerable<string> GetArguments()
        {
            return new List<string>();
        }
    }
}
