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
        IClass node)
    {
        return true;
    }

    public bool Check(
        RecognizerContext ctx,
        INode node)
    {
        if (node is not IClass classEntity)
        {
            throw new NotImplementedException("Class Check was incorrect");
        }

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
                            method))
                        {
                            hasMatch = true;
                            break;
                        }
                    }
                    if (!hasMatch)
                    {
                        return false;
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
                            field))
                        {
                            hasMatch = true;
                            break;
                        }
                    }
                    if (!hasMatch)
                    {
                        return false;
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
                            constructor))
                        {
                            hasMatch = true;
                            break;
                        }
                    }
                    if (!hasMatch)
                    {
                        return false;
                    }
                    break;
                }
                case NotCheck:
                {
                    if (!check.Check(
                        ctx,
                        node))
                    {
                        return false;
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
        Console.WriteLine($"Got class '{classEntity}'");
        throw new NotImplementedException("Class Check was correct");

    }
}
