using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

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
        public BlockSyntax GetBody() => accesor.Body;
        public SyntaxTokenList GetModifiers() => property.Modifiers;
        public string GetName() => property.Identifier.ToString();
        public string GetReturnType() => property.Type.ToString();
    }
}
