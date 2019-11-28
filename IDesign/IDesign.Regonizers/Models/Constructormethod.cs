using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using IDesign.Checks;

namespace IDesign.Recognizers
{
    public class Constructormethod : IMethod
    {
        public Constructormethod(ConstructorDeclarationSyntax constructor)
        {
            this.Constructor = constructor;
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
    }
}