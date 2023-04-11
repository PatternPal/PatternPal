namespace PatternPal.Core.Checks;

internal class CheckCollection : ICheck
{
    private readonly CheckCollectionKind _checkCollectionKind;
    private readonly IList< ICheck > _checks;

    internal CheckCollection(
        CheckCollectionKind checkCollectionKind,
        IList< ICheck > checks)
    {
        _checkCollectionKind = checkCollectionKind;
        _checks = checks;
    }

    bool ICheck.Check(
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
                        node))
                    {
                        return false;
                    }
                }

                return true;
            }
            case CheckCollectionKind.Any:
            {
                foreach (ICheck check in _checks)
                {
                    if (check.Check(
                        ctx,
                        node))
                    {
                        return true;
                    }
                }

                return false;
            }
            default:
                throw new NotImplementedException();
        }
    }
}
