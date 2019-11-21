using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Regonizers.Abstractions
{
    interface IRecognizer
    {
        IResult Recognize(IEntityNode node);
    }
}
