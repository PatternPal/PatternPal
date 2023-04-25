namespace PatternPal.Core;

// TODO CV: Make immutable to force a reset of the properties on each check level.
internal class RecognizerContext
{
    internal SyntaxGraph Graph { get; init; }
    internal IEntity CurrentEntity { get; set; }
    internal MethodCheck ParentCheck { get; set; }
}
