using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Recognizers.Abstractions
{
   public interface IResourceMessage
    {
         string GetKey();
        string[] GetParameters();
    }
}
