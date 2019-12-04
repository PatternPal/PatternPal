using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers.Models
{
   public enum RelationType
    {
        Implements,
        ImplementedBy,
        Extends,
        ExtendedBy,
        Uses,
        UsedBy,
        Creates,
        CreatedBy
    }
}
