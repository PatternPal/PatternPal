#region

using System.Linq;
using SyntaxTree;
using SyntaxTree.Abstractions;
using SyntaxTree.Abstractions.Root;
using static PatternPal.Core.Builders.CheckBuilder;

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
        RecognizerContext ctx = new() { Graph = graph };

        //the UsesFunction node from the syntax graph 
        INode usesNode = graph.GetAll()["Uses"].GetMethods().FirstOrDefault(x => x.GetName() == "UsesFunction");
        //the UsedFunction node from the syntax graph 
        INode usedNode = graph.GetAll()["Used"].GetMethods().FirstOrDefault(x => x.GetName() == "UsedFunction");

        //check for the UsedFunction
        MethodCheckBuilder usedMethod = Method(Priority.Low);
        //checking the check to set MatchedEntity to UsedFunction
        usedMethod.Build().Check(ctx, usedNode); 

        //check for the UsesFunction referring to the UsedFunction
        MethodCheck usesMethod = (MethodCheck)Method(Priority.Low, Uses(Priority.Low, usedMethod.Result)).Build();
        
        ICheckResult result = usesMethod.Check(ctx, usesNode);

        return Verifier.Verify(result);
    }
     
    [Test]
    public Task Uses_Check_Returns_Correct_True_Result_For_Entity()
    {
        //a syntax graph with a uses relation from Uses to UsedFunction
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        RecognizerContext ctx = new() { Graph = graph };

        //the UsesFunction node from the syntax graph 
        INode usesNode = graph.GetAll()["Uses"];
        //the UsedFunction node from the syntax graph 
        INode usedNode = graph.GetAll()["Used"].GetMethods().FirstOrDefault(x => x.GetName() == "UsedFunction");

        //check for the UsedFunction
        MethodCheckBuilder usedMethod = Method(Priority.Low);
        //checking the check to set MatchedEntity to UsedFunction
        usedMethod.Build().Check(ctx, usedNode);

        //check for Uses referring to the UsedFunction
        ClassCheck usesClass = (ClassCheck)Class(Priority.Low, Uses(Priority.Low, usedMethod.Result)).Build();

        ICheckResult result = usesClass.Check(ctx, usesNode);

        return Verifier.Verify(result);
    }

    
    [Test]
    public Task Uses_Check_Returns_Correct_False_Result()
    {
       //a syntax graph with a uses relation from UsesFunction to UsedFunction
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        RecognizerContext ctx = new() { Graph = graph };

        //the UsesFunction node from the syntax graph 
        INode usesNode = graph.GetAll()["Uses"].GetMethods().FirstOrDefault(x => x.GetName() == "UsesFunction");
        //the UsedFunction node from the syntax graph 
        INode usedNode = graph.GetAll()["Used"].GetMethods().FirstOrDefault(x => x.GetName() == "UsedFunction");

        //check to find the UsesFunction
        MethodCheckBuilder usesMethod = Method(Priority.Low);
        //checking the check to set MatchedEntity to UsesFunction
        usesMethod.Build().Check(ctx, usesNode);

        //check for the UsedFunction referring to the UsesFunction, a relation which does not exist
        MethodCheck usedMethod = (MethodCheck)Method(Priority.Low, Uses(Priority.Low, usesMethod.Result)).Build();

        ICheckResult result = usedMethod.Check(ctx, usedNode);

        return Verifier.Verify(result);
    }
}
