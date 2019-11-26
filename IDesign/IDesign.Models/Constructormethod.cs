using Microsoft.CodeAnalysis;
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

        public BlockSyntax GetBody()
        {
            return constructor.Body;
        }

        public SyntaxTokenList GetModifiers()
        {
            return constructor.Modifiers;
        }

        public string GetName()
        {
            return constructor.Identifier.ToString();
        }

        public string GetReturnType()
        {
            return "void";
        }
    }
}