using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Root;
using SyntaxTree.Models.Root;

namespace SyntaxTree {
    public class SyntaxGraph {
        private readonly Dictionary<string, IEntity> _all = new Dictionary<string, IEntity>();
        private readonly List<IRoot> _roots = new List<IRoot>();

        private readonly Relations _relations;

        public SyntaxGraph() {
            _relations = new Relations(this);
        }

        /// <summary>
        /// Add a file to the graph
        /// </summary>
        /// <param name="content">The content of the file</param>
        /// <param name="source">The source of the file</param>
        /// <returns>The parsed file</returns>
        public IRoot AddFile(string content, string source) {
            var tree = CSharpSyntaxTree.ParseText(content);
            var root = new Root(tree.GetCompilationUnitRoot(), source, this);
            
            _roots.Add(root);
            foreach (var pair in root.GetAllEntities()) {
                if (!_all.ContainsKey(pair.Key)) {
                    _all.Add(pair.Key, pair.Value);
                }
            }

            return root;
        }

        public IEnumerable<IRoot> GetRoots() => _roots.AsReadOnly();
        public Dictionary<string, IEntity> GetAll() => new Dictionary<string, IEntity>(_all);

        /// <summary>
        ///     Creates all relations between classes
        /// </summary>
        public void CreateGraph() {
            _relations.Reset();
            _relations.CreateEdgesOfEntities();
        }

        /// <summary>
        ///     Get all relations that a entity has
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns>A wrapper of the entity with the relations</returns>
        public IEnumerable<IRelation> GetRelations(IEntity entity) {
            if (!_relations.relations.ContainsKey(entity)) return Array.Empty<IRelation>();
            return _relations.relations[entity].AsReadOnly();
        }
    }
}
