namespace PatternPal.Core.Checks;

internal class CheckCollection : CheckBase
{
    private readonly CheckCollectionKind _checkCollectionKind;
    private readonly IList< ICheck > _checks;

    internal CheckCollection(Priority priority,
        CheckCollectionKind checkCollectionKind,
        IList< ICheck > checks) : base(priority)
    {
        _checkCollectionKind = checkCollectionKind;
        _checks = checks;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        // TODO: Run check recursively on _checks and decide result based on _checkCollectionKind.
        switch (_checkCollectionKind)
        {
            case CheckCollectionKind.All:
            {
                foreach (ICheck check in _checks)
                {
                    if (!check.Check(
                            ctx,
                            node).Correctness)
                    {
                        throw new NotImplementedException("Check Check was incorrect");
                    }
                }

                throw new NotImplementedException("Check Check was correct");
            }
            case CheckCollectionKind.Any:
            {
                foreach (ICheck check in _checks)
                {
                    if (check.Check(
                        ctx,
                        node).Correctness)
                    {
                        throw new NotImplementedException("CheckCollection check was correct");
                    }
                }

                throw new NotImplementedException("CheckCollection check was incorrect");
                }
            default:
                throw new NotImplementedException();
        }
    }
}
