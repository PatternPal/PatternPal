﻿#region

using System.Linq;
using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Utils;

#endregion

namespace PatternPal.SyntaxTree.Models.Root
{
    /// <summary>
    /// A root represents a compiled file in the syntax graph.
    /// </summary>
    public class Root : AbstractNode, IRoot
    {
        // The entities present in the root/file.
        private readonly List<IEntity> _entities;
        // The graph in which the root resides.
        private readonly SyntaxGraph _graph;
        // The namespaces present in the root.
        private readonly List<Namespace> _namespaces;
        // The name of the source file.
        private readonly string _source;
        // The libraries used.
        private readonly List<UsingDirectiveSyntax> _using;

        /// <summary>
        /// Returns an instance of <see cref="Root"/>.
        /// </summary>
        public Root(CompilationUnitSyntax node, string source, SyntaxGraph graph) : base(node, null)
        {
            _source = source;
            _graph = graph;

            _using = node.Usings.ToList();
            _namespaces = node.Members.OfType<BaseNamespaceDeclarationSyntax>()
                .Select(name => new Namespace(name, this))
                .ToList();

            _entities = node.Members.OfType<TypeDeclarationSyntax>()
                .Select(type => type.ToEntity(this))
                .ToList();
        }

        /// <inheritdoc />
        public override string GetName() { return "root"; }

        /// <inheritdoc />
        public string GetSource() { return _source; }

        /// <inheritdoc />
        public new IRoot GetRoot() { return this; }

        /// <inheritdoc />
        public IEnumerable<INamespace> GetNamespaces() { return _namespaces.AsReadOnly(); }

        /// <inheritdoc />
        public IEnumerable<UsingDirectiveSyntax> GetUsing() { return _using.AsReadOnly(); }

        /// <summary>
        /// Get all entities from only this root.
        /// </summary>
        public IEnumerable<IEntity> GetEntities() { return _entities.AsReadOnly(); }

        /// <summary>
        /// Get all entities from roots used, recursively.
        /// </summary>
        public Dictionary<string, IEntity> GetAllEntities()
        {
            return GetNamespaces()
                .Select(ns => ns.GetAllEntities())
                .Concat(GetEntities().OfType<IEntitiesContainer>().Select(e => e.GetAllEntities()))
                .Append(GetEntities().ToDictionary(e => e.GetFullName()))
                .SelectMany(d => d)
                .ToDictionary(p => p.Key, p => p.Value);
        }

        /// <inheritdoc />
        public IEnumerable<Relation> GetRelations(INode node, RelationTargetKind type)
        {
            return _graph.GetRelations(node, type);
        }

        /// <inheritdoc />
        public IEnumerable<INode> GetChildren()
        {
            return _entities
                .Cast<INode>()
                .Concat(_namespaces);
        }
    }
}
