using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers.Abstractions
{
    public interface IRecognizer
    {
        IResult Recognize(IEntityNode node);
    }
}
