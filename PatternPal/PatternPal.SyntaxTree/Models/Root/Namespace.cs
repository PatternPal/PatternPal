using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Utils;

namespace PatternPal.SyntaxTree.Models.Root
{
    public class Namespace : AbstractNode, INamespace
    {
        private readonly List<IEntity> _entities;

        private readonly List<Namespace> _namespaces;
        private readonly INamespaceContainer _parent;
        private readonly BaseNamespaceDeclarationSyntax _syntax;
        private readonly List<UsingDirectiveSyntax> _using;

        public Namespace(BaseNamespaceDeclarationSyntax node, INamespaceContainer parent) : base(
            node, parent?.GetRoot()
        )
        {
            _syntax = node;
            _parent = parent;

            _using = node.Usings.ToList();
            _namespaces = node.Members.OfType<BaseNamespaceDeclarationSyntax>()
                .Select(name => new Namespace(name, this))
                .ToList();

            _entities = node.Members.OfType<TypeDeclarationSyntax>()
                .Select(type => type.ToEntity(this))
                .ToList();
        }

        public override string GetName()
        {
            return GetNamespace();
        }

        public INamespaceContainer GetParent()
        {
            return _parent;
        }

        public string GetNamespace()
        {
            if (_parent is INamedEntitiesContainer name)
            {
                return $"{name}.{_syntax.Name.ToString()}";
            }

            return _syntax.Name.ToString();
        }

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

        public IEnumerable<INode> GetChildren()
        {
            return _entities
                .Cast<INode>()
                .Concat(_namespaces);
        }
    }
}
