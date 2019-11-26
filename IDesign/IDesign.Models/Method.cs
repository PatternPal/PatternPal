using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public class Method : IMethod
    {
        public Method(MethodDeclarationSyntax method)
        {
            this.method = method;
        }


        public MethodDeclarationSyntax method { get; set; }

        public BlockSyntax GetBody()
        {
            return method.Body;
        }

        public SyntaxTokenList GetModifiers()
        {
            return method.Modifiers;
        }

        public string GetName()
        {
            return method.Identifier.ToString();
        }

        public string GetReturnType()
        {
            return method.ReturnType.ToString();
        }
    }
}