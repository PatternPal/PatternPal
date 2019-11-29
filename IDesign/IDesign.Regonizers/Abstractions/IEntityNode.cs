using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace IDesign.Recognizers.Abstractions
{
    public interface IEntityNode
    {
        TypeDeclarationSyntax GetTypeDeclarationSyntax();
        string GetName();
        IEnumerable<IMethod> GetMethods();
        IEnumerable<IField> GetFields();
        IEnumerable<IMethod> GetConstructors();
        IEnumerable<IRelation> GetRelations();
    }
}