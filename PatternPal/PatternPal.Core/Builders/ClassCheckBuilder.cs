namespace PatternPal.Core.Builders;

internal class ClassCheckBuilder : CheckCollectionBuilder
{
    private MethodCheckBuilder ? _methodCheckBuilder;

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
        // TODO CV: Can we define the implementing type on the interface so we don't have to specify the types explicitly.
        return ICheckBuilder.BuildCheck(
            this,
            ref _methodCheckBuilder,
            builderAction);
    }
}
