using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;
using PatternPal.Recognizers.Abstractions;

namespace PatternPal.CommonResources;

public static class ResourceUtils
{
    private static ResourceManager _resourceMan;
    private static readonly Regex Regex = new(@"\{\d+\}");

    public static ResourceManager ResourceManager
    {
        get
        {
            if (_resourceMan is not null)
            {
                return _resourceMan;
            }

            ResourceManager temp = new(
                "PatternPal.CommonResources.ClassFeedbackRes", typeof(ClassFeedbackRes).Assembly
            );
            _resourceMan = temp;

            return _resourceMan;
        }
    }

    public static CultureInfo Culture { get; set; }

    public static string GetResourceFromString(string name)
    {
        return ResourceManager.GetString(name, Culture);
    }

    public static string ResourceMessageToString(IResourceMessage resMessage, ICheckResult result = null)
    {
        string message = "";

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
        string res = "";
        IResourceMessage messageResource = result.GetFeedback();

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

        string resMessageString = GetResourceFromString(resMessage.GetKey());

        return resMessageString != null && Regex.IsMatch(resMessageString);
    }
}
