namespace PatternPal.Core.Checks;

internal class OperatorCheck : ICheck
{
    private readonly TypeOperatorCheck _typeOperatorCheck;
    private readonly List<ICheck> _childChecks;
    public OperatorCheck(TypeOperatorCheck typeOperatorCheck, List<ICheck> childChecks)
    {
        _typeOperatorCheck = typeOperatorCheck;
        _childChecks = childChecks;
    }

    //Todo: check _childCheckBuilders and return result with lists of results of children 
    bool ICheck.Check(INode node)
    {
        switch (_typeOperatorCheck)
        {
            case TypeOperatorCheck.All:
                throw new NotImplementedException();
                break;
            case TypeOperatorCheck.Any:
                throw new NotImplementedException();
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
