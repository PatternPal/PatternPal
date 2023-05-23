using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.Tests.Utils;

internal class RecognizerContext4Tests : IRecognizerContext
{
    public bool IsRootContext { get; }
    public SyntaxGraph Graph { get; }
    public IEntity CurrentEntity { get; }
    public ICheck ParentCheck { get; }
    public ICheck EntityCheck { get;  }

    private RecognizerContext4Tests(
        SyntaxGraph graph,
        ICheck rootCheck)
    {
        IsRootContext = true;
        Graph = graph;
        CurrentEntity = null!;
        ParentCheck = rootCheck;
    }

    internal static RecognizerContext4Tests Empty() => new(
        null!,
        null! );

    internal static RecognizerContext4Tests Create(
        SyntaxGraph graph,
        ICheck ? rootCheck = null) => new(
        graph,
        rootCheck! );
}
