using System.Globalization;
using System.Resources;
using IDesign.Recognizers.Abstractions;

namespace IDesign.CommonResources
{
    public static class ResourceUtils
    {
        private static ResourceManager resourceMan;

        public static ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan is null)
                {
                    var temp = new ResourceManager(
                        "IDesign.CommonResources.ClassFeedbackRes", typeof(ClassFeedbackRes).Assembly
                    );
                    resourceMan = temp;
                }

                return resourceMan;
            }
        }

        public static CultureInfo Culture { get; set; }

        public static string GetResourceFromString(string name)
        {
            return ResourceManager.GetString(name, Culture);
        }

        public static string ResourceMessageToString(IResourceMessage ResMessage)
        {
            var message = "";
            if (ResMessage != null)
            {
                message = GetResourceFromString(ResMessage.GetKey());
                if (ResMessage.GetParameters() != null && ResMessage.GetParameters().Length > 0)
                {
                    message = string.Format(message, ResMessage.GetParameters());
                }

                return message;
            }

            return message;
        }

        public static string ResultToString(ICheckResult result)
        {
            var res = "";
            if (result.GetElement() != null)
            {
                res += result.GetElement() + " | ";
            }

            if (result.GetFeedback() != null)
            {
                res += ResourceMessageToString(result.GetFeedback());
            }

            return res;
        }
    }
}
