using IDesign.Recognizers.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace IDesign.Recognizers.Abstractions
{
    public interface IEntityNode
    {
        /// <summary>
        ///     Gets the TypeDeclarationSyntax of this class
        /// </summary>
        TypeDeclarationSyntax GetTypeDeclarationSyntax();

        /// <summary>
        ///     Gets the name of this class
        /// </summary>
        string GetName();

        /// <summary>
        ///     Gets the sourcefile of this class
        /// </summary>
        string GetSourceFile();

        /// <summary>
        ///     Gets the methods of this class
        /// </summary>
        IEnumerable<IMethod> GetMethods();

        /// <summary>
        ///     Gets the fields of this class
        /// </summary>
        IEnumerable<IField> GetFields();

        /// <summary>
        ///     Gets the constructors of this class
        /// </summary>
        IEnumerable<IMethod> GetConstructors();

        /// <summary>
        ///     Gets the relations of this class
        /// </summary>
        IList<IRelation> GetRelations();

        /// <summary>
        ///     Gets the type of this class
        /// </summary>
        EntityNodeType GetEntityNodeType();

    }
}