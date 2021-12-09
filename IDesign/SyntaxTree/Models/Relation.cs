using Microsoft.CodeAnalysis;
using SyntaxTree.Abstractions;

namespace SyntaxTree.Models {
    public class Relation : IRelation {
        public Relation(IEntityNode entityNode, RelationType type) {
            EntityNode = entityNode;
            Type = type;
        }

        public IEntityNode EntityNode { get; set; }
        public RelationType Type { get; set; }

        public IEntityNode GetDestination() {
            return EntityNode;
        }

        public RelationType GetRelationType() {
            return Type;
        }

        public string GetSuggestionName() {
            return GetDestination().GetSuggestionName();
        }

        public SyntaxNode GetSuggestionNode() {
            return GetDestination().GetSuggestionNode();
        }
    }
}
