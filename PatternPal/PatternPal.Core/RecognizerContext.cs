using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Abstractions.Entities;

namespace PatternPal.Core;

// TODO CV: Make immutable to force a reset of the properties on each check level.
internal class RecognizerContext
{
    internal SyntaxGraph Graph { get; init; }
    internal IEntity CurrentEntity { get; init; }
    internal MethodCheck ParentCheck { get; set; }
}
