namespace PatternPal.Core.Checks;

internal interface ICheck
{
    bool Check(
        RecognizerContext ctx,
        INode node);
}
