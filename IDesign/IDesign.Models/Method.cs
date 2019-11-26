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
        public BlockSyntax GetBody() => method.Body;
        public SyntaxTokenList GetModifiers() => method.Modifiers;
        public string GetName() => method.Identifier.ToString();
        public TypeSyntax GetReturnType() => method.ReturnType;
    }
}
