using Microsoft.CodeAnalysis;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Entities;
using SyntaxTree.Abstractions.Root;

namespace SyntaxTree.Models.Entities {
    public class Relation : IRelation {
        public Relation(IEntity entityNode, RelationType type) {
            _entityNode = entityNode;
            _type = type;
        }

        private readonly IEntity _entityNode;
        private readonly RelationType _type;

        public IEntity GetDestination() => _entityNode;

        public RelationType GetRelationType() => _type;

        public string GetName() => _entityNode.GetName();

        public SyntaxNode GetSyntaxNode() => _entityNode.GetSyntaxNode();

        public IRoot GetRoot() => _entityNode.GetRoot();
    }
}
