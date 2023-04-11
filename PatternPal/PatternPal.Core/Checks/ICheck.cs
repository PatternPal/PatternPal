namespace PatternPal.Core.Checks;

internal interface ICheck
{
    ICheckResult Check(
        RecognizerContext ctx,
        INode node);
}
