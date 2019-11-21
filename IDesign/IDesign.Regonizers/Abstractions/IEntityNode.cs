using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Regonizers.Abstractions
{
    public interface IEntityNode
    {
        TypeDeclarationSyntax GetTypeDeclarationSyntax();

        string GetName();
        IEnumerable<ConstructorDeclarationSyntax> GetCostructors();
        IEnumerable<MethodDeclarationSyntax> GetMethods();
        IEnumerable<PropertyDeclarationSyntax> GetProperties();
    }
}
