namespace IDesign.Recognizers.Abstractions
{
    public interface IResourceMessage
    {
        string GetKey();
        string[] GetParameters();
    }
}
