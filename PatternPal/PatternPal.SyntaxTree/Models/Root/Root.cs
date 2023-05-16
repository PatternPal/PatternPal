using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Root
{
    public class Root : AbstractNode, IRoot
    {
        private readonly List<IEntity> _entities;
        private readonly SyntaxGraph _graph;
        private readonly List<Namespace> _namespaces;
        private readonly string _source;
        private readonly List<UsingDirectiveSyntax> _using;

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

        public override string GetName() { return "root"; }

        public string GetSource() { return _source; }

        public new IRoot GetRoot() { return this; }

        public IEnumerable<INamespace> GetNamespaces() { return _namespaces.AsReadOnly(); }
        public IEnumerable<UsingDirectiveSyntax> GetUsing() { return _using.AsReadOnly(); }
        public IEnumerable<IEntity> GetEntities() { return _entities.AsReadOnly(); }

        public Dictionary<string, IEntity> GetAllEntities()
        {
            return GetNamespaces()
                .Select(ns => ns.GetAllEntities())
                .Concat(GetEntities().OfType<IEntitiesContainer>().Select(e => e.GetAllEntities()))
                .Append(GetEntities().ToDictionary(e => e.GetFullName()))
                .SelectMany(d => d)
                .ToDictionary(p => p.Key, p => p.Value);
        }

        public IEnumerable<Relation> GetRelations(INode node, RelationTargetKind type)
        {
            return _graph.GetRelations(node, type);
        }
        
        public IEnumerable<INode> GetChildren()
        {
            return _entities
                .Cast<INode>()
                .Concat(_namespaces);
        }
    }
}
