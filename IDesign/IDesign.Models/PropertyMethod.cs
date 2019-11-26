using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Models
{
    public class PropertyMethod : IMethod
    {
        public PropertyMethod(PropertyDeclarationSyntax property, AccessorDeclarationSyntax accesor)
        {
            this.property = property;
            this.accesor = accesor;
        }

        public PropertyDeclarationSyntax property { get; set; }
        public AccessorDeclarationSyntax accesor { get; set; }

        public BlockSyntax GetBody()
        {
            return accesor.Body;
        }

        public SyntaxTokenList GetModifiers()
        {
            return property.Modifiers;
        }

        public string GetName()
        {
            return property.Identifier.ToString();
        }

        public string GetReturnType()
        {
            return property.Type.ToString();
        }
    }
}