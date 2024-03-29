﻿namespace PatternPal.Tests.Utils;

internal class RecognizerContext4Tests : IRecognizerContext
{
    public bool IsRootContext { get; }
    public SyntaxGraph Graph { get; }
    public IEntity CurrentEntity { get; }
    public ICheck ParentCheck { get; }
    public ICheck EntityCheck { get;  }
    public IRecognizerContext ? PreviousContext { get; }

    private RecognizerContext4Tests(
        SyntaxGraph graph,
        ICheck rootCheck,
        IEntity entity)
    {
        IsRootContext = true;
        Graph = graph;
        CurrentEntity = entity;
        ParentCheck = rootCheck;
    }

    internal static RecognizerContext4Tests Empty() => new(
        null!,
        null!, 
        null!);

    internal static RecognizerContext4Tests Create(
        SyntaxGraph graph,
        ICheck ? rootCheck = null) => new(
        graph,
        rootCheck!,
        null!);

    internal static RecognizerContext4Tests WithEntity(IEntity entity) => new(
        null!, null!, entity);
}
