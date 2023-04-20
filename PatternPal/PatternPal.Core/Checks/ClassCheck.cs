namespace PatternPal.Core.Checks;

internal class ClassCheck : CheckBase
{
    private readonly IEnumerable< ICheck > _checks;

    internal ClassCheck(Priority priority, 
        IEnumerable<ICheck> checks) : base(priority)
    {
        _checks = checks;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        IClass classEntity = CheckHelper.ConvertNodeElseThrow< IClass >(node);

        bool hasFailedSubCheck = false;
        IList< ICheckResult > subCheckResults = new List< ICheckResult >();
        foreach (ICheck check in _checks)
        {
            switch (check)
            {
                case ModifierCheck modifierCheck:
                {
                    ICheckResult subCheckResult = modifierCheck.Check(
                        ctx,
                        classEntity);
                    subCheckResults.Add(subCheckResult);
                    hasFailedSubCheck = hasFailedSubCheck || !subCheckResult.Correctness;
                    break;
                }
                case MethodCheck methodCheck:
                {
                    bool hasMatch = false;
                    foreach (IMethod method in classEntity.GetMethods())
                    {
                        if (methodCheck.Check(
                            ctx,
                            method).Correctness)
                        {
                            hasMatch = true;
                            break;
                        }
                    }
                    if (!hasMatch)
                    {
                        throw new NotImplementedException("Class Check was incorrect");
                    }
                    break;
                }
                case FieldCheck fieldCheck:
                {
                    bool hasMatch = false;
                    foreach (IField field in classEntity.GetFields())
                    {
                        if (fieldCheck.Check(
                            ctx,
                            field).Correctness)
                        {
                            hasMatch = true;
                            break;
                        }
                    }
                    if (!hasMatch)
                    {
                        throw new NotImplementedException("Class Check was incorrect");
                    }
                    break;
                }
                case ConstructorCheck constructorCheck:
                {
                    bool hasMatch = false;
                    foreach (IConstructor constructor in classEntity.GetConstructors())
                    {
                        if (constructorCheck.Check(
                            ctx,
                            constructor).Correctness)
                        {
                            hasMatch = true;
                            break;
                        }
                    }
                    if (!hasMatch)
                    {
                        throw new NotImplementedException("Class Check was incorrect");
                    }
                    break;
                }
                case NotCheck:
                {
                    if (!check.Check(
                        ctx,
                        node).Correctness)
                    {
                        throw new NotImplementedException("Class Check was incorrect");
                    }

                    break;
                }
                case UsesCheck usesCheck:
                {
                    ICheckResult subCheckResult = usesCheck.Check(
                        ctx,
                        classEntity);
                    subCheckResults.Add(subCheckResult);
                    hasFailedSubCheck = hasFailedSubCheck || !subCheckResult.Correctness;
                    break;
                }
                default:
                    throw CheckHelper.InvalidSubCheck(
                        this,
                        check);
            }
        }

        return new NodeCheckResult
               {
                   ChildrenCheckResults = subCheckResults,
                   Correctness = !hasFailedSubCheck,
                   FeedbackMessage = $"Found class '{classEntity}'",
                   Priority = Priority.Knockout,
               };
    }
}
