﻿namespace PatternPal.Core.Checks;

/// <summary>
/// Checks for a method of an entity, depending on the list of <see cref="_checks"/> provided.
/// The checks performed can be a collection of <see cref="TypeCheck"/>s, <see cref="ModifierCheck"/>s,
/// <see cref="ParameterCheck"/>s, <see cref="UsesCheck"/>s, etc.
/// </summary>
internal class MethodCheck : CheckBase
{
    //A list of checks needed to perform on the method.
    private readonly IEnumerable<ICheck> _checks;

    //A list of all found instances
    internal List<INode> MatchedEntities { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="checks">A list of checks needed to perform on the method.</param>
    internal MethodCheck(
        Priority priority,
        IEnumerable<ICheck> checks)
        : base(priority)
    {
        MatchedEntities = new List<INode>();
        _checks = checks;
    }

    internal Func<List<INode>> Result => () => MatchedEntities;

    /// <summary>
    /// This method executes all the given checks on the <paramref name="node"/>
    /// </summary>
    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        IMethod methodEntity = CheckHelper.ConvertNodeElseThrow<IMethod>(node);

        IList<ICheckResult> subCheckResults = new List<ICheckResult>();
        foreach (ICheck check in _checks)
        {
            switch (check)
            {
                case ModifierCheck modifierCheck:
                    {
                        subCheckResults.Add(
                            modifierCheck.Check(
                                ctx,
                                methodEntity));
                        break;
                    }
                case TypeCheck typeCheck:
                    {
                        //TODO return type needs to be obtained from the methodEntity
                        throw new NotImplementedException();
                    }
                case NotCheck:
                    {
                        throw new NotImplementedException("Method Check was incorrect");
                    }
                case UsesCheck usesCheck:
                    {
                        subCheckResults.Add(
                            usesCheck.Check(
                                ctx,
                                methodEntity));
                        break;
                    }
                case ParameterCheck parameterCheck:
                    {
                        //TODO check parameters of methods
                        throw new NotImplementedException();
                    }
                default:
                    throw CheckHelper.InvalidSubCheck(
                        this,
                        check);
            }
        }

        // Add the checked method to the list of found methods
        MatchedEntities.Add(methodEntity);

        return new NodeCheckResult
        {
            ChildrenCheckResults = subCheckResults,
            Priority = Priority,
            FeedbackMessage = $"Found method: {methodEntity}."
        };
    }
}
