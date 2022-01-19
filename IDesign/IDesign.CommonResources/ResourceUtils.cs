using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;
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

        public static string ResourceMessageToString(IResourceMessage resMessage, ICheckResult result)
        {
            var message = "";

            if (resMessage != null)
            {
                message = GetResourceFromString(resMessage.GetKey());

                if (resMessage.GetParameters() != null && resMessage.GetParameters().Length > 0)
                {
                    message = string.Format(message, resMessage.GetParameters());
                }
                else if (ContainsCurlyBracketsWithDigits(resMessage))
                {
                    message = string.Format(message, result.GetElement());
                }

                return message;
            }

            return message;
        }

        public static string ResultToString(ICheckResult result)
        {
            var res = "";
            var messageResource = result.GetFeedback();

            if (result.GetElement() != null && !ContainsCurlyBracketsWithDigits(messageResource))
            {
                res += result.GetElement() + " | ";
            }

            if (messageResource != null)
            {
                res += ResourceMessageToString(messageResource, result);
            }

            return res;
        }

        private static bool ContainsCurlyBracketsWithDigits(IResourceMessage resMessage)
        {
            if (resMessage != null)
            {
                var resMessageString = GetResourceFromString(resMessage?.GetKey());
                var regex = new Regex(@"\{\d\}");

                if (resMessageString != null)
                {
                    return regex.IsMatch(resMessageString);
                }
            }

            return false;
        }
    }
}
