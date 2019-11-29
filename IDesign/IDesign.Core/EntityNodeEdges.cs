using IDesign.Recognizers.Abstractions;
using IDesign.Recognizers.Models;

namespace IDesign.Core
{
    public class EntityNodeEdges : IRelation
    {
        public EntityNodeEdges(EntityNode entityNode) 
        {
            EntityNode = entityNode;
        }

        public EntityNode EntityNode { get; set; }

        public IEntityNode GetDestination()
        {
            return EntityNode;
        }

        RelationType IRelation.GetType()
        {
            throw new System.NotImplementedException();
        }
    }
}