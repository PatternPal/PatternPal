namespace PatternPal.Core.Builders;

internal class ClassCheckBuilder : CheckCollectionBuilder
{
    public ClassCheckBuilder()
        : base(CheckCollectionKind.All)
    {
    }

    public ClassCheckBuilder(
        IEnumerable< ICheckBuilder > checkBuilders)
        : base(
            CheckCollectionKind.All,
            checkBuilders)
    {
    }

    public override ICheck Build() => new ClassCheck();

    internal ClassCheckBuilder Not(
        Action< ClassCheckBuilder > builderAction)
    {
        base.Not(builderAction);
        return this;
    }

    internal ClassCheckBuilder Not(
        ICheckBuilder checkBuilder)
    {
        CheckBuilders.Add(new NotCheckBuilder(checkBuilder));
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

    internal ClassCheckBuilder Any(
        params ICheckBuilder[ ] checkBuilders)
    {
        CheckBuilders.Add(
            new CheckCollectionBuilder(
                CheckCollectionKind.Any,
                checkBuilders));
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
