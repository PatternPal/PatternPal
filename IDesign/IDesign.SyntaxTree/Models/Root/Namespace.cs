using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Root;
using SyntaxTree.Utils;

namespace SyntaxTree.Models.Root {
    public class Namespace : AbstractNode, INamespace {
        private readonly IRoot _parent;
        private readonly BaseNamespaceDeclarationSyntax _syntax;
        
        private readonly List<Namespace> _namespaces;
        private readonly List<IEntity> _entities;
        private readonly List<UsingDirectiveSyntax> _using;

        public Namespace(BaseNamespaceDeclarationSyntax node, IRoot parent) : base(node, parent.GetRoot()) {
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

        public override string GetName() => GetNamespace();
        public string GetSource() => _parent.GetSource();
        public IRoot GetParent() => _parent;

        public string GetNamespace() {
            if (_parent is INamespace name) return $"{name}.{_syntax.Name.ToString()}";
            return _syntax.Name.ToString();
        }

        public IEnumerable<INamespace> GetNamespaces() { return _namespaces.AsReadOnly(); }
        public IEnumerable<UsingDirectiveSyntax> GetUsing() { return _using.AsReadOnly(); }
        public IEnumerable<IEntity> GetEntities() { return _entities.AsReadOnly(); }
        
        public Dictionary<string, IEntity> GetAllEntities() {
            Dictionary<string, IEntity> dic = GetNamespaces().Select(ns => ns.GetAllEntities())
                .SelectMany(d => d)
                .ToDictionary(p => p.Key, p => p.Value);

            foreach (var entity in GetEntities()) {
                dic.Add(entity.GetFullName(), entity);
            }
            
            return dic;
        }

        public IEnumerable<IRelation> GetRelations(IEntity entity) => _parent.GetRelations(entity);
    }
}
