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
        IEnumerable<IMethod> GetMethods();
        IEnumerable<IField> GetFields();

        IEnumerable<IMethod> GetConstructors();
    }
}
