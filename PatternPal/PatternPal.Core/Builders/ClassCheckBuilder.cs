namespace PatternPal.Core.Builders;

internal class ClassCheckBuilder : CheckCollectionBuilder
{
    public ClassCheckBuilder()
        : base(CheckCollectionKind.All)
    {
    }

    public override ICheck Build() => new ClassCheck();

    internal ClassCheckBuilder Not(
        Action< ClassCheckBuilder > builderAction)
    {
        base.Not(builderAction);
        return this;
    }

    internal ClassCheckBuilder Any(
        params Action< ClassCheckBuilder >[ ] builderActions)
    {
        List< ICheckBuilder > builders = new( builderActions.Length );
        foreach (Action< ClassCheckBuilder > builderAction in builderActions)
        {
            ClassCheckBuilder nestedBuilder = new();
            builderAction(nestedBuilder);
            builders.AddRange(nestedBuilder.CheckBuilders);
        }

        CheckBuilders.Add(
            new CheckCollectionBuilder(
                CheckCollectionKind.Any,
                builders));
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
