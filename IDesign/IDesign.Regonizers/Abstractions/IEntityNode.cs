using System.Collections.Generic;
using IDesign.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Recognizers.Abstractions
{
    public interface IEntityNode
    {
        TypeDeclarationSyntax GetTypeDeclarationSyntax();
        string GetName();
        IEnumerable<IMethod> GetMethods();
        IEnumerable<IField> GetFields();

        IEnumerable<IMethod> GetConstructors();
    }
}