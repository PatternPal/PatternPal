using Microsoft.CodeAnalysis;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Models
{
    public abstract class AbstractNode : INode
    {
        private readonly SyntaxNode _node;
        private readonly IRoot _root;

        protected AbstractNode(SyntaxNode node, IRoot root)
        {
            _node = node;
            _root = root;
        }

        public abstract string GetName();

        public SyntaxNode GetSyntaxNode()
        {
            return _node;
        }

        public IRoot GetRoot()
        {
            return _root;
        }

        public override string ToString() { return GetName(); }
    }
}
