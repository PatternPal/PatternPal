#region

using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;

using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Root;
using SyntaxTree.Models.Members.Method;
using SyntaxTree.Models.Root;

#endregion

namespace SyntaxTree
{
    public class SyntaxGraph
    {
        private readonly Dictionary< string, IEntity > _all = new Dictionary< string, IEntity >();

        private readonly Relations _relations;
        private readonly List< IRoot > _roots = new List< IRoot >();

        public SyntaxGraph()
        {
            _relations = new Relations(this);
        }

        /// <summary>
        ///     Add a file to the graph
        /// </summary>
        /// <param name="content">The content of the file</param>
        /// <param name="source">The source of the file</param>
        /// <returns>The parsed file</returns>
        public IRoot AddFile(
            string content,
            string source)
        {
            Microsoft.CodeAnalysis.SyntaxTree tree = CSharpSyntaxTree.ParseText(content);
            Root root = new Root(
                tree.GetCompilationUnitRoot(),
                source,
                this);

            _roots.Add(root);
            foreach (KeyValuePair< string, IEntity > pair in root.GetAllEntities())
            {
                if (!_all.ContainsKey(pair.Key))
                {
                    _all.Add(
                        pair.Key,
                        pair.Value);
                }
            }

            return root;
        }

        public IEnumerable< IRoot > GetRoots()
        {
            return _roots.AsReadOnly();
        }

        /// <summary>
        /// Indicates whether this <see cref="SyntaxGraph"/> contains any entities.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="SyntaxGraph"/> contains no entities.</returns>
        public bool IsEmpty => _all.Count == 0;

        public Dictionary< string, IEntity > GetAll()
        {
            return new Dictionary< string, IEntity >(_all);
        }

        /// <summary>
        ///     Creates all relations between classes
        /// </summary>
        public void CreateGraph()
        {
            _relations.Reset();
            _relations.CreateEdges();
        }

        /// <summary>
        ///     Get all relations that a node has
        /// </summary>
        /// <param name="node">The node</param>
        /// <returns>A wrapper of the entity with the relations</returns>
        public IEnumerable<Relation> GetRelations(INode node)
        {
            switch (node)
            {
                case IEntity e:
                    if (_relations.EntityRelations.TryGetValue(e, out List<Relation> result))
                        return result;
                    break;
                case Method m:
                    if(_relations.MethodRelations.TryGetValue(m, out List<Relation> result2))
                        return result2;
                    break;
            }

            return new List<Relation>();
        }
    }
}
