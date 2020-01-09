using IDesign.CommonResources;
using IDesign.Recognizers.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.CommonResources
{
    public static class ResourceUtils
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;
        public static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("IDesign.CommonResources.ClassFeedbackRes", typeof(ClassFeedbackRes).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        public static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }
        public static string GetResourceFromString(string name)
        {
            return ResourceManager.GetString(name, resourceCulture);
        }

        public static string ResourceMessageToString(IResourceMessage ResMessage)
        {
            string message = "";
            if (ResMessage == null)
                return message;

            message = GetResourceFromString(ResMessage.GetKey());
            if (ResMessage.GetParameters() != null && ResMessage.GetParameters().Length > 0)
            {
                message = string.Format(message, ResMessage.GetParameters());
            }
            return message;
        }

        public static string ResultToString(ICheckResult result)
        {
            var res = "";
            if (result.GetFeedback() == null) 
                return res;
            if(result.GetElement() != null)
                res += result.GetElement().GetSuggestionName() + " | ";
            res += ResourceMessageToString(result.GetFeedback());

            return res;
        }
    }
}
