using Microsoft.CodeAnalysis.VisualBasic;
using PatternPal.Recognizers.Models.Checks;

namespace PatternPal.Core.Checks;

internal class FieldCheck : CheckBase
{
    private readonly IEnumerable<ICheck> _checks;
    internal FieldCheck(Priority priority, 
        IEnumerable<ICheck> checks) : base(priority)
    {
        _checks = checks;
    }

    public override ICheckResult Check(RecognizerContext ctx, INode node)
    {
        if (node is not IField field)
        {
            throw new NotImplementedException("Field check was incorrect");
        }

        foreach (ICheck check in _checks) 
        {
            switch (check)
            {
                case ModifierCheck modifierCheck:
                    {
                        bool hasMatch = false;
                        foreach (IModified modified in field.GetModifiers())
                        {
                            if (modifierCheck.Check
                                (ctx,
                                modified).Correctness)
                            {
                                hasMatch = true;
                                break;
                            }
                        }
                        if (!hasMatch)
                        {
                            throw new NotImplementedException("Modifier check was incorrect");
                        }
                        break;
                    }
                case TypeCheck typeCheck:
                    {
                        if (!check.Check(ctx, node).Correctness)
                        {
                            throw new NotImplementedException("Type check was incorrect");
                        }
                        break;
                    }
                case FieldCheck fieldCheck:
                    {
                        throw new NotImplementedException("Field check is not yet implemented");
                    }
                default:
                    {
                        Console.WriteLine($"Unexpected check: {check.GetType().Name}");
                        break;
                    }
            }
            //Kan weg denk ik
            //if (!check.Check(ctx, node).Correctness) throw new NotImplementedException("Field Check was incorrect");
        }

        Console.WriteLine($"Got field {field}");
        throw new NotImplementedException("Field check was correct");
    }
}
