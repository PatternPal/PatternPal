namespace PatternPal.Core.Checks;

internal class ClassCheck : ICheck
{
    private readonly IEnumerable< ICheck > _checks;

    internal ClassCheck(
        IEnumerable< ICheck > checks)
    {
        _checks = checks;
    }

    public ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        IClass classEntity = CheckHelper.ConvertAndThrowIfIncorrectNodeType< IClass >(node);

        foreach (ICheck check in _checks)
        {
            switch (check)
            {
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
                default:
                {
                    Console.WriteLine($"Unexpected check: {check.GetType().Name}");
                    break;
                }
            }
        }

        return new NodeCheckResult
               {
                   ChildrenCheckResults = new List< ICheckResult >(),
                   Correctness = true,
                   FeedbackMessage = $"Found class '{classEntity}'",
                   Priority = Priority.Knockout,
               };
    }
}
