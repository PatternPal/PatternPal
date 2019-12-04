using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;

namespace IDesign.Core
{
    public class EntityNodeEdges : IRelation
    {
        public EntityNodeEdges(IEntityNode entityNode,RelationType type)
        {
            EntityNode = entityNode;
            Type = type;
        }

        public IEntityNode EntityNode { get; set; }
        public RelationType Type { get; set; }

        public IEntityNode GetDestination()
        {
            throw new System.NotImplementedException();
        }

        RelationType IRelation.GetType()
        {
            return Type;
        }
    }
}