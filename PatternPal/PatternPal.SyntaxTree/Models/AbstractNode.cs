using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Root;

namespace PatternPal.SyntaxTree.Models
{
    /// <summary>
    /// Base implementation of a Node.
    /// </summary>
    public abstract class AbstractNode : INode
    {
        // The Roslyn representation of the node.
        private readonly SyntaxNode _node;
        // The root in which the node resides.
        private readonly IRoot _root;

        /// <summary>
        /// Returns an instance of <see cref="AbstractNode"/>.
        /// </summary>
        protected AbstractNode(SyntaxNode node, IRoot root)
        {
            _node = node;
            _root = root;
        }

        /// <inheritdoc />
        public abstract string GetName();

        /// <summary>
        /// Returns the Roslyn representation of the <see cref="INode"/>.
        /// </summary>
        public SyntaxNode GetSyntaxNode()
        {
            return _node;
        }

        /// <summary>
        /// Returns the <see cref="IRoot"/> this node belongs to.
        /// </summary>
        public IRoot GetRoot()
        {
            return _root;
        }

        public override string ToString() { return GetName(); }
    }
}
