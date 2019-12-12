using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;

namespace IDesign.Core.Models
{
    public class Relation : IRelation
    {
        public Relation(IEntityNode entityNode, RelationType type)
        {
            EntityNode = entityNode;
            Type = type;
        }

        public IEntityNode EntityNode { get; set; }
        public RelationType Type { get; set; }

        public IEntityNode GetDestination()
        {
            return EntityNode;
        }

        public RelationType GetRelationType()
        {
            return Type;
        }
    }
}