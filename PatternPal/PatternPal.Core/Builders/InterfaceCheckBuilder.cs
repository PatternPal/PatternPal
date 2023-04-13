namespace PatternPal.Core.Builders;

internal class InterfaceCheckBuilder : CheckCollectionBuilder
{

    internal InterfaceCheckBuilder(IEnumerable<ICheckBuilder> checkBuilders) : base(CheckCollectionKind.All, checkBuilders)
    {
    }

    public override ICheck Build() => new InterfaceCheck(CheckBuilders.Select(checkBuilder => checkBuilder.Build()));
}

