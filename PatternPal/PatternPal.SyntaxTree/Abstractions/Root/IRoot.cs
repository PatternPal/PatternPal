﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.SyntaxTree.Abstractions.Root
{
    /// <summary>
    /// A root represents a compiled file in the syntax graph.
    /// </summary>
    public interface IRoot : IEntitiesContainer, INamespaceContainer, IUsingContainer
    {
        /// <summary>
        /// Get the filepath of the source file where this node is found in.
        /// </summary>
        string GetSource();

        IEnumerable<Relation> GetRelations(INode node, RelationTargetKind type);
    }

    /// <summary>
    /// Represents a node which has namespaces.
    /// </summary>
    public interface INamespaceContainer : INode, IParent
    {
        /// <summary>
        /// Get all Namespaces in the current node.
        /// </summary>
        IEnumerable<INamespace> GetNamespaces();
    }

    /// <summary>
    /// Represents a node which has usings.
    /// </summary>
    public interface IUsingContainer : INode
    {
        /// <summary>
        /// Get all usings in the current node.
        /// </summary>
        IEnumerable<UsingDirectiveSyntax> GetUsing();
    }

    /// <summary>
    /// Represents a node which has entities.
    /// </summary>
    public interface IEntitiesContainer : INode, IParent
    {
        /// <summary>
        /// Get all <see cref="IEntity"/>s in the current <see cref="INode"/>, not in sub namespaces.
        /// </summary>
        IEnumerable<IEntity> GetEntities();

        /// <summary>
        /// Get all <see cref="IEntity"/>s mapped by their fullname.
        /// This looks in sub namespaces.
        /// </summary>
        Dictionary<string, IEntity> GetAllEntities();
    }

    public interface INamedEntitiesContainer : IEntitiesContainer
    {
        string GetNamespace();
    }
}
