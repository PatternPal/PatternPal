﻿namespace PatternPal.Core.Builders;

enum CheckCollectionKind
{
    All,
    Any
}

internal class CheckCollectionBuilder : ICheckBuilder
{
    private readonly CheckCollectionKind _checkCollectionKind;
    protected readonly IList< ICheckBuilder > CheckBuilders;

    internal CheckCollectionBuilder(
        CheckCollectionKind checkCollectionKind)
    {
        _checkCollectionKind = checkCollectionKind;
        CheckBuilders = new List< ICheckBuilder >();
    }

    internal CheckCollectionBuilder(
        CheckCollectionKind checkCollectionKind,
        IEnumerable< ICheckBuilder > checkBuilders)
    {
        _checkCollectionKind = checkCollectionKind;
        CheckBuilders = checkBuilders as List< ICheckBuilder > ?? new List< ICheckBuilder >(checkBuilders);
    }

    public virtual ICheck Build()
    {
        IList< ICheck > checks = new List< ICheck >(CheckBuilders.Count);
        foreach (ICheckBuilder checkBuilder in CheckBuilders)
        {
            checks.Add(checkBuilder.Build());
        }

        return new CheckCollection(
            _checkCollectionKind,
            checks);
    }
}