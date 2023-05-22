#region

using System.Linq;

using Microsoft.CodeAnalysis.CSharp;

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Models.Root;

#endregion

namespace PatternPal.SyntaxTree
{
    /// <summary>
    /// A SyntaxGraph is a graphical representation of a codebase, including <see cref="INode"/>s and <see cref="Relation"/>s.
    /// </summary>
    public class SyntaxGraph
    {
        // A dictionary to search for an entity by its name.
        private readonly Dictionary< string, IEntity > _all = new();

        /// <summary>
        /// All relations between <see cref="IEntity"/>s and <see cref="IMember"/>s in the syntax graph.
        /// </summary>
        public Relations Relations { get; }
        // A root represents a compiled file in the syntax graph
        private readonly List< IRoot > _roots = new();

        /// <summary>
        /// Returns an instance of <see cref="SyntaxGraph"/>.
        /// </summary>
        public SyntaxGraph()
        {
            Relations = new Relations(this);
            SemanticModels.Reset();
        }

        /// <summary>
        /// Add a file to the <see cref="SyntaxGraph"/>.
        /// </summary>
        /// <param name="content">The content of the file</param>
        /// <param name="source">The source of the file</param>
        /// <returns>The parsed file an an instance of <see cref="IRoot"/></returns>
        public IRoot AddFile(
            string content,
            string source)
        {
            Microsoft.CodeAnalysis.SyntaxTree tree = CSharpSyntaxTree.ParseText(content);
            Root root = new(
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

        /// <summary>
        /// Gets all the <see cref="IRoot"/>s, thus all file representations, of the <see cref="SyntaxGraph"/>.
        /// </summary>
        /// <returns>All the <see cref="IRoot"/>s of the <see cref="SyntaxGraph"/></returns>
        public IEnumerable< IRoot > GetRoots()
        {
            return _roots.AsReadOnly();
        }

        /// <summary>
        /// Indicates whether this <see cref="SyntaxGraph"/> contains any entities.
        /// </summary>
        /// <returns><see langword="true"/> if this <see cref="SyntaxGraph"/> contains no entities.</returns>
        public bool IsEmpty => _all.Count == 0;

        /// <summary>
        /// Returns all <see cref="IEntity"/>s of the <see cref="SyntaxGraph"/> searchable by their name.
        /// </summary>
        /// <returns>All <see cref="IEntity"/>s of the <see cref="SyntaxGraph"/></returns>
        public Dictionary< string, IEntity > GetAll()
        {
            return new Dictionary< string, IEntity >(_all);
        }

        /// <summary>
        /// Finish the <see cref="SyntaxGraph"/> by creating all relations between classes.
        /// </summary>
        public void CreateGraph()
        {
            Relations.Reset();
            Relations.CreateEdges();
        }

        /// <summary>
        /// Gets all <see cref="Relation"/>s that an <see cref="INode"/> has, filtered on the type of the destination node of the relation.
        /// </summary>
        /// <param name="node">The <see cref="INode"/> from which you want to know the relations</param>
        /// <param name="type">The type of the destination <see cref="INode"/></param>
        /// <returns>An IEnumerable of relations for this <see cref="INode"/> filtered on type</returns>
        public IEnumerable< Relation > GetRelations(
            INode node,
            RelationTargetKind type)
        {
            List< Relation > ? relations = null;
            switch (node)
            {
                case IEntity entity:
                    Relations.EntityRelations.TryGetValue(
                        entity,
                        out relations);
                    break;
                case IMember member:
                    Relations.MemberRelations.TryGetValue(
                        member,
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
                RelationTargetKind.Member => relations.Where(x => x.Target.IsT1),
                _ => relations
            };
        }
    }
}
