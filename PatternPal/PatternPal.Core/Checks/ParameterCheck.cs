using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PatternPal.Core.Checks;

/// <summary>
/// Checks the parameters of the entity node method 
/// </summary>
internal class ParameterCheck : CheckBase
{
    // The types the parameters should have.
    private readonly IEnumerable< TypeCheck > _parameterTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterCheck"/> class.
    /// </summary>
    /// <param name="priority">Priority of the check.</param>
    /// <param name="parameterTypes">A list of types the node method should have.</param>
    internal ParameterCheck(
        Priority priority,
        IEnumerable< TypeCheck > parameterTypes)
        : base(priority)
    {
        _parameterTypes = parameterTypes;
    }

    public override ICheckResult Check(
        RecognizerContext ctx,
        INode node)
    {
        // Cast node to IMethod.
        IMethod method = CheckHelper.ConvertNodeElseThrow<IMethod>(node);

        // The parameters the method has.
        List<TypeSyntax> methods = method.GetParameters().ToList();

        foreach (TypeSyntax parameterType in methods)
        {

        }
    }
}
