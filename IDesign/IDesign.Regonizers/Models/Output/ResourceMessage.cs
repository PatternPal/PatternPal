using System;
using System.Collections.Generic;
using System.Text;
using IDesign.Recognizers.Abstractions;

namespace IDesign.Recognizers.Models.Output
{
   public class ResourceMessage : IResourceMessage
    {
        public string Key;
        public string[] Parameters;

        public ResourceMessage(string key, string[] parameters)
        {
            Key = key;
            Parameters = parameters;
        }
        public ResourceMessage(string key)
        {
            Key = key;
        }
        public string GetKey()
        {
            return Key;
        }

        public string[] GetParameters()
        {
            return Parameters;
        }
    }
}
