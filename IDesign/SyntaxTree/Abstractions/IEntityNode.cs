using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Models;

namespace SyntaxTree.Abstractions {
    public interface IEntityNode : INode {
        /// <summary>
        ///     Get the declaration syntax of the node.
        ///     Possible syntax types: ClassDeclarationSyntax, InterfaceDeclarationSyntax.
        /// </summary>
        /// <returns>The declaration syntax of the node</returns>
        TypeDeclarationSyntax GetTypeDeclarationSyntax();

        /// <summary>
        ///     Get the name of the node.
        ///     Could be the name of a class or a interface.
        /// </summary>
        /// <returns>The name of the node</returns>
        string GetName();

        /// <summary>
        ///     Get the filepath of the source file where this node is found in
        /// </summary>
        /// <returns>Filepath of the source</returns>
        string GetSourceFile();

        /// <summary>
        ///     Get a list of methods declared in this node
        /// </summary>
        /// <returns>A list of methods</returns>
        IEnumerable<IMethod> GetMethodsAndProperties();

        /// <summary>
        ///     Get a list of fields declared in this node
        /// </summary>
        /// <returns>A list of fields</returns>
        IEnumerable<IField> GetFields();

        /// <summary>
        ///     Get a list of constructors declared in this node
        /// </summary>
        /// <returns>A list of constructors</returns>
        IEnumerable<IMethod> GetConstructors();

        /// <summary>
        ///     Gets the relations of this class
        /// </summary>
        IEnumerable<IRelation> GetRelations();

        /// <summary>
        ///     Gets the type of this class
        /// </summary>
        EntityNodeType GetEntityNodeType();

        /// <summary>
        ///     Gets te modifiers of this class
        /// </summary>
        /// <returns></returns>
        SyntaxTokenList GetModifiers();
    }
}
