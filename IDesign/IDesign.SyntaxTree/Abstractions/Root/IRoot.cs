using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions.Entities;

namespace SyntaxTree.Abstractions.Root {
    public interface IRoot: INode {
        /// <summary>
        ///     Get all Namespaces in the current node
        /// </summary>
        /// <returns>List of Namespaces</returns>
        IEnumerable<INamespace> GetNamespaces();
        
        /// <summary>
        ///     Get the filepath of the source file where this node is found in
        /// </summary>
        /// <returns>Filepath of the source</returns>
        string GetSource();

        IEnumerable<UsingDirectiveSyntax> GetUsing();
        
        /// <summary>
        ///     Get all entities in the current node, not in sub namespaces!
        /// </summary>
        /// <returns>List of notes</returns>
        IEnumerable<IEntity> GetEntities();
        
        /// <summary>
        ///     Get all entities mapped by their fullname
        ///     This looks in sub namespaces!
        /// </summary>
        /// <returns>A dictionary mapped with fullname</returns>
        Dictionary<string, IEntity> GetAllEntities();

        IEnumerable<IRelation> GetRelations(IEntity entity);
    }
}
