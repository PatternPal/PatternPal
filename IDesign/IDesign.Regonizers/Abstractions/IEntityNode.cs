using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace IDesign.Recognizers.Abstractions
{
    public interface IEntityNode
    {
        TypeDeclarationSyntax GetTypeDeclarationSyntax();
        string GetName();
        string GetSourceFile();
        IEnumerable<IMethod> GetMethods();
        IEnumerable<IField> GetFields();
        IEnumerable<IMethod> GetConstructors();
        IList<IRelation> GetRelations();
        EntityNodeType GetEntityNodeType();
    }
}