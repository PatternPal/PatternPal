using System.Collections.Generic;
using System.Linq;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Models
{
    public class PropertyMethod : IMethod
    {
        public PropertyMethod(PropertyDeclarationSyntax property)
        {
            Property = property;
        }

        public PropertyDeclarationSyntax Property { get; set; }
        public BlockSyntax GetBody() {
            return Property.AccessorList?.Accessors
                .Where(s => s.Kind() == SyntaxKind.GetAccessorDeclaration)
                .Select(s => s.Body)
                .First();
        }

        public SyntaxTokenList GetModifiers()
        {
            return Property.Modifiers;
        }

        public string GetName()
        {
            return Property.Identifier.ToString();
        }

        public IEnumerable<ParameterSyntax> GetParameters()
        {
            return new List<ParameterSyntax>();
        }

        public IEnumerable<string> GetParameterTypes()
        {
            return new List<string>();
        }

        public ParameterListSyntax GetParameter()
        {
            return null;
        }

        public string GetReturnType()
        {
            return Property.Type.ToString();
        }

        public string GetSuggestionName()
        {
            return $"{GetName()}_get";
        }

        public SyntaxNode GetSuggestionNode()
        {
            return Property;
        }

        public IEnumerable<string> GetArguments()
        {
            return new List<string>();
        }
    }
}