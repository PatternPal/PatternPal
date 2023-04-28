#region
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class ParameterCheckTests
{
    [Test]
    public void Parameter_Check_Method_No_Parameters()
    {
        // Create graph, two classes with types. Method in class "Used" has three parameters two
        // of those are identical types.
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        RecognizerContext ctx = new() { Graph = graph };

        // Obtain the uses class from syntax graph
        IEnumerable<IMethod> usesNode = graph.GetAll()["Uses"].GetMethods();
        // Obtain the used class from syntax graph
        IEnumerable<IMethod> usedNode = graph.GetAll()["Used"].GetMethods();

        // Maak methodcheck waarin je geeft de node de method met als return type de parameter type
        // die je wil checken.
        // MethodCheck { TypeCheck { Methode met als return type used } }

        // Maak tweede methodcheck, refereer naar 

        // Maak een parametercheck met de result van 

        MethodCheck parameterCheck = Method(Priority.Low);
        parameterCheck.Check(
            ctx,
            );

    }
}
