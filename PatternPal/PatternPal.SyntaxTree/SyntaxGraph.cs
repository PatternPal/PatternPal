#region

using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;

using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Root;
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
            SemanticModels.Reset();
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

            SemanticModels.AddTreesToCompilation(tree);

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
        ///     Get all relations that a node has, filtered on the type of the destination node of the relation.
        /// </summary>
        /// <param name="node">The node</param>
        /// <param name="type">The type of the destination node</param>
        /// <returns>An IEnumerable of relations for this node filtered on type</returns>
        public IEnumerable< Relation > GetRelations(
            INode node,
            RelationTargetKind type)
        {
            List< Relation > ? relations = null;
            switch (node)
            {
                case IEntity entity:
                    _relations.EntityRelations.TryGetValue(
                        entity,
                        out relations);
                    break;
                case IMethod method:
                    _relations.MethodRelations.TryGetValue(
                        method,
                        out relations);
                    break;
            }

            if (relations is null)
            {
                return new List< Relation >();
            }

            return type switch
            {
                RelationTargetKind.Entity => relations.Where(x => x.Target.IsT0),
                RelationTargetKind.Method => relations.Where(x => x.Target.IsT1),
                _ => relations
            };
        }
    }
}
