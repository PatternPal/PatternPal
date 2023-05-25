namespace PatternPal.Recognizers.Abstractions;

public interface IResourceMessage
{
    string GetKey();
    string[] GetParameters();
}
