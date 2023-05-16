namespace PatternPal.Tests;

public class NodeConverter : JsonConverter
{
    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        INode ? node = value as INode;
        serializer.Serialize(
            writer,
            node?.GetName());
    }

    public override object ? ReadJson(
        JsonReader reader,
        Type type,
        object ? existingValue,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(
        Type type)
    {
        // Check if the `type` we received is derived from `INode`.
        Type nodeType = typeof( INode );
        return nodeType.IsAssignableFrom(type);
    }

    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.AddExtraSettings(_ => _.Converters.Add(new NodeConverter()));
    }
}
