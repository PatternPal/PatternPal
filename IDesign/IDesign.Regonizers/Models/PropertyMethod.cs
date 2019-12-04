using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers
{
    public class PropertyMethod : IMethod
    {
        public PropertyMethod(PropertyDeclarationSyntax property, AccessorDeclarationSyntax accesor)
        {
            this.Property = property;
            this.Accesor = accesor;
        }

        public PropertyDeclarationSyntax Property { get; set; }
        public AccessorDeclarationSyntax Accesor { get; set; }

        public BlockSyntax GetBody()
        {
            return Accesor.Body;
        }

        public SyntaxTokenList GetModifiers()
        {
            return Property.Modifiers;
        }

        public string GetName()
        {
            return Property.Identifier.ToString();
        }

        public string GetReturnType()
        {
            return Property.Type.ToString();
        }

        public string GetSuggestionName()
        {
            return GetName() + "_get";
        }

        public SyntaxNode GetSuggestionNode()
        {
            return Property;
        }
    }
}