using IDesign.Recognizers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers.Abstractions
{
    public interface IRelation
    {
        /// <summary>
        ///     Gets the destination node of this relation
        /// </summary>
        IEntityNode GetDestination();
        /// <summary>
        ///     Gets the type of this relation
        /// </summary>
        RelationType GetRelationType();
    }
}
