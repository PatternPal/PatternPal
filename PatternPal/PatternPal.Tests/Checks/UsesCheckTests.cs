#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class UsesCheckTests
{
    [Test]
    public Task Uses_Check_Returns_Correct_True_Result_For_Method()
    {
        //a syntax graph with a uses relation from UsesFunction to UsedFunction
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        RecognizerContext ctx = new()
                                {
                                    Graph = graph
                                };

        //the UsesFunction node from the syntax graph 
        INode usesNode = graph.GetAll()[ "Uses" ].GetMethods().FirstOrDefault(x => x.GetName() == "UsesFunction");
        //the UsedFunction node from the syntax graph 
        INode usedNode = graph.GetAll()[ "Used" ].GetMethods().FirstOrDefault(x => x.GetName() == "UsedFunction");

        //check for the UsedFunction
        MethodCheck usedMethod = Method(Priority.Low);
        //checking the check to set MatchedEntity to UsedFunction
        usedMethod.Check(
            ctx,
            usedNode);

        //check for the UsesFunction referring to the UsedFunction
        MethodCheck usesMethod = Method(
            Priority.Low,
            Uses(
                Priority.Low,
                usedMethod.Result));

        ICheckResult result = usesMethod.Check(
            ctx,
            usesNode);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Uses_Check_Returns_Correct_True_Result_For_Entity()
    {
        //a syntax graph with a uses relation from Uses to UsedFunction
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        RecognizerContext ctx = new()
                                {
                                    Graph = graph
                                };

        //the UsesFunction node from the syntax graph 
        INode usesNode = graph.GetAll()[ "Uses" ];
        //the UsedFunction node from the syntax graph 
        INode usedNode = graph.GetAll()[ "Used" ].GetMethods().FirstOrDefault(x => x.GetName() == "UsedFunction");

        //check for the UsedFunction
        MethodCheck usedMethod = Method(Priority.Low);
        //checking the check to set MatchedEntity to UsedFunction
        usedMethod.Check(
            ctx,
            usedNode);

        //check for Uses referring to the UsedFunction
        ClassCheck usesClass = Class(
            Priority.Low,
            Uses(
                Priority.Low,
                usedMethod.Result));

        ICheckResult result = usesClass.Check(
            ctx,
            usesNode);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Uses_Check_Returns_Correct_False_Result()
    {
        //a syntax graph with a uses relation from UsesFunction to UsedFunction
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        RecognizerContext ctx = new()
                                {
                                    Graph = graph
                                };

        //the UsesFunction node from the syntax graph 
        INode usesNode = graph.GetAll()[ "Uses" ].GetMethods().FirstOrDefault(x => x.GetName() == "UsesFunction");
        //the UsedFunction node from the syntax graph 
        INode usedNode = graph.GetAll()[ "Used" ].GetMethods().FirstOrDefault(x => x.GetName() == "UsedFunction");

        //check to find the UsesFunction
        MethodCheck usesMethod = Method(Priority.Low);
        //checking the check to set MatchedEntity to UsesFunction
        usesMethod.Check(
            ctx,
            usesNode);

        //check for the UsedFunction referring to the UsesFunction, a relation which does not exist
        MethodCheck usedMethod = Method(
            Priority.Low,
            Uses(
                Priority.Low,
                usesMethod.Result));

        ICheckResult result = usedMethod.Check(
            ctx,
            usedNode);

        return Verifier.Verify(result);
    }
}
