using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Core
{
    public class EntityNodeEdges
    {
        public EntityNode EntityNode { get; set; }

        public EntityNodeEdges(EntityNode entityNode)
        {
            this.EntityNode = entityNode;
        }
    }
}
