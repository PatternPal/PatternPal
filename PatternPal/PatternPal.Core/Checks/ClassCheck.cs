namespace PatternPal.Core.Checks;

internal class ClassCheck : ICheck
{
    private readonly IEnumerable< ICheck > _checks;

    internal ClassCheck(
        IEnumerable< ICheck > checks)
    {
        _checks = checks;
    }

    public bool Check(
        RecognizerContext ctx,
        INode node)
    {
        if (node is not IClass classEntity)
        {
            return false;
        }

        Console.WriteLine($"Got class '{classEntity}'");
        foreach (ICheck check in _checks)
        {
            switch (check)
            {
                case MethodCheck methodCheck:
                {
                    foreach (IMethod method in classEntity.GetMethods())
                    {
                        methodCheck.Check(ctx, method);
                    }
                    break;
                }
                case FieldCheck fieldCheck:
                {
                    foreach (IField field in classEntity.GetFields())
                    {
                        fieldCheck.Check(ctx, field);
                    }
                    break;
                }
                case ConstructorCheck constructorCheck:
                {
                    foreach (IConstructor constructor in classEntity.GetConstructors())
                    {
                        constructorCheck.Check(ctx, constructor);
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
        return true;
    }
}
