namespace PatternPal.Core.Checks;

internal class ClassCheck : CheckBase
{
    private readonly IEnumerable< ICheck > _checks;

    internal ClassCheck(
        Priority priority,
        IEnumerable< ICheck > checks)
        : base(priority)
    {
        _checks = checks;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        IClass classEntity = CheckHelper.ConvertNodeElseThrow< IClass >(node);

        ctx.CurrentEntity = classEntity;

        IList< ICheckResult > subCheckResults = new List< ICheckResult >();
        foreach (ICheck check in _checks)
        {
            switch (check)
            {
                case ModifierCheck modifierCheck:
                {
                    subCheckResults.Add(
                        modifierCheck.Check(
                            ctx,
                            classEntity));
                    break;
                }
                case MethodCheck methodCheck:
                {
                    List< ICheckResult > methodCheckResults = new();
                    foreach (IMethod method in classEntity.GetMethods())
                    {
                        methodCheckResults.Add(
                            methodCheck.Check(
                                ctx,
                                method));
                    }
                    subCheckResults.Add(
                        new NodeCheckResult
                        {
                            ChildrenCheckResults = methodCheckResults,
                            CollectionKind = CheckCollectionKind.Any,
                            FeedbackMessage = string.Empty,
                            Priority = methodCheck.Priority,
                        });
                    break;
                }
                case FieldCheck fieldCheck:
                {
                    List< ICheckResult > fieldCheckResults = new();
                    foreach (IField field in classEntity.GetFields())
                    {
                        fieldCheckResults.Add(
                            fieldCheck.Check(
                                ctx,
                                field));
                    }
                    subCheckResults.Add(
                        new NodeCheckResult
                        {
                            ChildrenCheckResults = fieldCheckResults,
                            CollectionKind = CheckCollectionKind.Any,
                            FeedbackMessage = string.Empty,
                            Priority = fieldCheck.Priority,
                        });
                    break;
                }
                case ConstructorCheck constructorCheck:
                {
                    List< ICheckResult > constructorCheckResults = new();
                    foreach (IConstructor constructor in classEntity.GetConstructors())
                    {
                        constructorCheckResults.Add(
                            constructorCheck.Check(
                                ctx,
                                constructor));
                    }
                    subCheckResults.Add(
                        new NodeCheckResult
                        {
                            ChildrenCheckResults = constructorCheckResults,
                            CollectionKind = CheckCollectionKind.Any,
                            FeedbackMessage = string.Empty,
                            Priority = constructorCheck.Priority,
                        });
                    break;
                }
                case NotCheck:
                {
                    break;
                    throw new NotImplementedException("Class Check was incorrect");
                }
                case UsesCheck usesCheck:
                {
                    subCheckResults.Add(
                        usesCheck.Check(
                            ctx,
                            classEntity));
                    break;
                }
                default:
                    throw CheckHelper.InvalidSubCheck(
                        this,
                        check);
            }
        }

        ctx.CurrentEntity = null;

        return new NodeCheckResult
               {
                   ChildrenCheckResults = subCheckResults,
                   FeedbackMessage = $"Found class '{classEntity}'",
                   Priority = Priority
               };
    }
}
