using PatternPal.Recognizers.Abstractions;

namespace PatternPal.Recognizers.Models.Output
{
    public class ResourceMessage : IResourceMessage
    {
        public string Key;
        public string[] Parameters;

        public ResourceMessage(string key, params string[] parameters)
        {
            Key = key;
            Parameters = parameters;
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
