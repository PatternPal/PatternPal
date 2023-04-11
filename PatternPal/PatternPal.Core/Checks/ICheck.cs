namespace PatternPal.Core.Checks;

internal interface ICheck
{
    CheckResult Check(
        RecognizerContext ctx,
        INode node);
}
