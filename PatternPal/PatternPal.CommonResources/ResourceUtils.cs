using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;
using PatternPal.Recognizers.Abstractions;

namespace PatternPal.CommonResources
{
    public static class ResourceUtils
    {
        private static ResourceManager resourceMan;
        private static readonly Regex regex = new Regex(@"\{\d+\}");

        public static ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan is null)
                {
                    var temp = new ResourceManager(
                        "PatternPal.CommonResources.ClassFeedbackRes", typeof(ClassFeedbackRes).Assembly
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

        public static string ResourceMessageToString(IResourceMessage resMessage, ICheckResult result = null)
        {
            var message = "";

            if (resMessage == null) return message;

            message = GetResourceFromString(resMessage.GetKey());
            if (message == null) return "";

            if (resMessage.GetParameters() != null && resMessage.GetParameters().Length > 0)
            {
                message = string.Format(message, resMessage.GetParameters());
            }
            else if (result != null && ContainsCurlyBracketsWithDigits(resMessage))
            {
                message = string.Format(message, result.GetElement());
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
            if (resMessage == null) return false;

            var resMessageString = GetResourceFromString(resMessage?.GetKey());

            if (resMessageString != null)
            {
                return regex.IsMatch(resMessageString);
            }

            return false;
        }
    }
}
