using IDesign.Recognizers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers.Abstractions
{
    public interface IRelation
    {
         IEntityNode GetDestination();
         RelationType GetType();
    }
}
