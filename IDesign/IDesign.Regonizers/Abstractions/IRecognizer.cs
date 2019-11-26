using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers.Abstractions
{
    interface IRecognizer
    {
        /// <summary>
        /// Analyses the given node for this pattern 
        /// </summary>
        /// <param name="entityNode">Enity node what it should check</param>
        /// <returns>The result object</returns>
        IResult Recognize(IEntityNode node);
    }
}
