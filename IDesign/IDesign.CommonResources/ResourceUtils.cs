using IDesign.Recognizers.Abstractions;

namespace IDesign.CommonResources
{
    public static class ResourceUtils
    {
        private static System.Resources.ResourceManager resourceMan;

        public static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan is null)
                {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("IDesign.CommonResources.ClassFeedbackRes", typeof(ClassFeedbackRes).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        public static System.Globalization.CultureInfo Culture { get; set; }
        public static string GetResourceFromString(string name)
        {
            return ResourceManager.GetString(name, Culture);
        }

        public static string ResourceMessageToString(IResourceMessage ResMessage)
        {
            string message = "";
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
            if (result.GetFeedback() != null)
            {
                if (result.GetElement() != null)
                {
                    res += result.GetElement().GetSuggestionName() + " | ";
                }

                res += ResourceMessageToString(result.GetFeedback());
            }
            return res;
        }
    }
}
