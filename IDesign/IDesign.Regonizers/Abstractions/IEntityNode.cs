using IDesign.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers.Abstractions
{
    public interface IEntityNode
    {
        TypeDeclarationSyntax GetTypeDeclarationSyntax();
        string GetName();
        IEnumerable<ConstructorDeclarationSyntax> GetCostructors();
        IEnumerable<IMethod> GetMethods();
        IEnumerable<PropertyDeclarationSyntax> GetProperties();
        IEnumerable<IField> GetFields();
    }
}
