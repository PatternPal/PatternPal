namespace PatternPal.Core.Builders;

internal class MethodCheckBuilder : CheckCollectionBuilder
{
    private readonly MethodCheck _check;

    internal MethodCheckBuilder()
        : base(CheckCollectionKind.All)
    {
        _check = new MethodCheck(CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
    }

    public override ICheck Build() => _check;

    internal MethodCheckBuilder Not(
        Action< MethodCheckBuilder > builderAction)
    {
        MethodCheckBuilder nestedBuilder = new();
        builderAction(nestedBuilder);
        CheckBuilders.Add(new NotCheckBuilder(nestedBuilder));
        return this;
    }

    internal MethodCheckBuilder Not(
        ICheckBuilder checkBuilder)
    {
        CheckBuilders.Add(new NotCheckBuilder(checkBuilder));
        return this;
    }

    internal MethodCheckBuilder Modifiers(
        params IModifier[ ] modifiers)
    {
        CheckBuilders.Add(new ModifierCheckBuilder(modifiers));
        return this;
    }

    internal MethodCheckBuilder Modifiers(
        Action< ModifierCheckBuilder > builderAction)
    {
        ModifierCheckBuilder modifierCheckBuilder = new();
        CheckBuilders.Add(modifierCheckBuilder);
        builderAction(modifierCheckBuilder);
        return this;
    }

    internal MethodCheckBuilder ReturnType(
        GetCurrentEntity getCurrentEntity)
    {
        return this;
    }

    internal Func< INode > Result => () => _check.MatchedEntity!;

    internal MethodCheckBuilder Uses(
        Func< INode > getNode)
    {
        CheckBuilders.Add(new UsesCheckBuilder(getNode));
        return this;
    }
}
