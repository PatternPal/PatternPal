namespace PatternPal.Core.Builders;

internal class InterfaceCheckBuilder : CheckCollectionBuilder
{

    internal InterfaceCheckBuilder(Priority priority, IEnumerable<ICheckBuilder> checkBuilders) : 
        base(priority, CheckCollectionKind.All, checkBuilders)
    {
    }

    public override ICheck Build() => new InterfaceCheck(Priority, CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
}

