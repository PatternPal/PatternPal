#region
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class ParameterCheckTests
{
    [Test]
    public Task Parameter_Check_Method_Same_Type()
    {
        // Create graph, two classes with types. Method in class "Used" has three parameters two
        // of those are identical types.
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        RecognizerContext ctx = new() { Graph = graph };

        // Obtain the uses method (0 parameters) from syntax graph
        IMethod usesNode = 
            graph.GetAll()["Uses"].GetMethods().FirstOrDefault(
                x => x.GetName() == "UsesFunction");
        // Obtain the used method (3 parameters) from syntax graph
        IMethod usedNode = 
            graph.GetAll()["Used"].GetMethods().FirstOrDefault(
                x => x.GetName() == "UsedFunction");

        // Type of the "UsesFunction" is "Uses"
        TypeCheck typeUsesNode = new TypeCheck(
            Priority.Low,
            OneOf<Func<List<INode>>, GetCurrentEntity>.FromT0(
                () => new List<INode> { usesNode }));
        // Type of the "UsesFunction" is "Used"
        TypeCheck typeUsedNode = new TypeCheck(
            Priority.Low,
            OneOf<Func<List<INode>>, GetCurrentEntity>.FromT0(
                () => new List<INode> { usedNode }));

        // Parameter check with two typechecks of type "Uses" and "Used"
        ParameterCheck usedParamCheck = 
            new ParameterCheck(Priority.Low, new List<TypeCheck>
            {
                typeUsesNode,
                typeUsedNode,
            });

        ICheckResult res = usedParamCheck.Check(ctx, usedNode);
        return Verifier.Verify(res);

        // TODO Vraag aan refactor team hoe dit werkt
        // Eerste methodcheck { typecheck { Method met type "Uses"
        // Maak tweede methodcheck, refereer naar vorige.
        // Maak een parametercheck met de result van ??

    }
}
