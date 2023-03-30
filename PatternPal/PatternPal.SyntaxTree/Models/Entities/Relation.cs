using Microsoft.CodeAnalysis;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Models.Entities
{
    public class Relation : IRelation<IEntity>
    {
        private readonly IEntity _entityNode;
        private readonly RelationType _type;

        public Relation(IEntity entityNode, RelationType type)
        {
            _entityNode = entityNode;
            _type = type;
        }

        public IEntity GetDestination()
        {
            return _entityNode;
        }

        public RelationType GetRelationType()
        {
            return _type;
        }

        public string GetName()
        {
            return _entityNode.GetName();
        }

        public SyntaxNode GetSyntaxNode()
        {
            return _entityNode.GetSyntaxNode();
        }

        public IRoot GetRoot()
        {
            return _entityNode.GetRoot();
        }

        public override string ToString()
        {
            return _entityNode.ToString();
        }
    }
}
