namespace PatternPal.Core.Builders;

internal class ClassCheckBuilder : CheckCollectionBuilder
{
    public ClassCheckBuilder()
        : base(CheckCollectionKind.All)
    {
    }

    public override ICheck Build() => new ClassCheck();

    internal ClassCheckBuilder Any(
        params ICheckBuilder[ ] checks)
    {
        CheckBuilders.Add(
            new CheckCollectionBuilder(
                CheckCollectionKind.Any,
                checks));
        return this;
    }

    internal ClassCheckBuilder Method(
        Action< MethodCheckBuilder > builderAction)
    {
        MethodCheckBuilder methodCheckBuilder = new();
        CheckBuilders.Add(methodCheckBuilder);
        builderAction(methodCheckBuilder);
        return this;
    }
}
