#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class RelationCheckTests
{
    [Test]
    public Task Uses_Check_Returns_Correct_True_Result_For_Method()
    {
        //a syntax graph with a uses relation from UsesFunction to UsedFunction
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

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
                usedMethod));

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
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        //the Uses class node from the syntax graph 
        INode usesNode = graph.GetAll()[ "Uses" ];
        //the UsedFunction node from the syntax graph 
        INode usedNode = EntityNodeUtils.GetMemberFromGraph<INode>(
            graph,
            "Used",
            "UsedFunction");

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
                usedMethod));

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
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        //the UsesFunction node from the syntax graph 
        INode usesNode = EntityNodeUtils.GetMemberFromGraph<INode>(
            graph,
            "Uses",
            "UsesFunction");
        //the UsedFunction node from the syntax graph 
        INode usedNode = EntityNodeUtils.GetMemberFromGraph<INode>(
            graph,
            "Used",
            "UsedFunction");

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
                usesMethod));

        ICheckResult result = usedMethod.Check(
            ctx,
            usedNode);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Inheritance_Check_Returns_Correct_True_Result()
    {
        //a syntax graph with an inheritance relation from Child to Parent
        SyntaxGraph graph = EntityNodeUtils.CreateInheritanceRelation();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        //the Child node from the syntax graph 
        INode childNode = graph.GetAll()[ "Child" ];
        //the Parent node from the syntax graph 
        INode parentNode = graph.GetAll()[ "Parent" ];

        //check for the Parent class
        ClassCheck parentClass = Class(Priority.Low);
        //checking the check to set MatchedEntity to parentNode
        parentClass.Check(
            ctx,
            parentNode);

        //check for the Child node inheriting from the Parent node
        ClassCheck childClass = Class(
            Priority.Low,
            Inherits(
                Priority.Low,
                parentClass));

        ICheckResult result = childClass.Check(
            ctx,
            childNode);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Inheritance_Check_Returns_Correct_False_Result()
    {
        //a syntax graph with no inheritance relation
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        //the Uses class node from the syntax graph 
        INode usesNode = graph.GetAll()[ "Uses" ];
        //the Used class node from the syntax graph 
        INode usedNode = graph.GetAll()[ "Used" ];

        //check to find the Uses class
        ClassCheck usesClass = Class(Priority.Low);
        //checking the check to set MatchedEntity to usesNode
        usesClass.Check(
            ctx,
            usesNode);

        //check for the UsedNode inheriting from the UsesNode, a relation which does not exist
        ClassCheck usedClass = Class(
            Priority.Low,
            Inherits(
                Priority.Low,
                usesClass));

        ICheckResult result = usedClass.Check(
            ctx,
            usedNode);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Implements_Check_Returns_Correct_True_Result()
    {
        //a syntax graph with an inheritance relation from Child to Parent
        SyntaxGraph graph = EntityNodeUtils.CreateImplementationRelation();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        //the Child node from the syntax graph 
        INode childInterfaceNode = graph.GetAll()[ "Child" ];
        //the Parent node from the syntax graph 
        INode parentInterfaceNode = graph.GetAll()[ "Parent" ];

        //check for the interface
        InterfaceCheck parentInterfaceCheck = Interface(Priority.Low);
        //checking the check to set MatchedEntity to interfaceNode
        parentInterfaceCheck.Check(
            ctx,
            parentInterfaceNode);

        //check for the Child node implementing the interface node
        InterfaceCheck childInterfaceCheck = Interface(
            Priority.Low,
            Implements(
                Priority.Low,
                parentInterfaceCheck));

        ICheckResult result = childInterfaceCheck.Check(
            ctx,
            childInterfaceNode);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Implements_Check_Returns_Correct_False_Result()
    {
        //a syntax graph with an inheritance relation from Child to Parent
        SyntaxGraph graph = EntityNodeUtils.CreateImplementationRelation();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        //the Child node from the syntax graph 
        INode childInterfaceNode = graph.GetAll()[ "Child" ];
        //the Parent node from the syntax graph 
        INode parentInterfaceNode = graph.GetAll()[ "Parent" ];

        //check for the child interface
        InterfaceCheck parentInterfaceCheck = Interface(Priority.Low);
        //checking the check to set MatchedEntity to interfaceNode
        parentInterfaceCheck.Check(
            ctx,
            childInterfaceNode);

        //check for the parent node implementing the interface node, which isnt the case
        InterfaceCheck childInterfaceCheck = Interface(
            Priority.Low,
            Implements(
                Priority.Low,
                parentInterfaceCheck));

        ICheckResult result = childInterfaceCheck.Check(
            ctx,
            parentInterfaceNode);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Creates_Check_Returns_Correct_True_Result()
    {
        //a syntax graph where the so called Uses class creates an instance of the so called Used class
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        //the Uses class node from the syntax graph 
        INode creatingNode = graph.GetAll()[ "Uses" ];
        //the Used class node from the syntax graph 
        INode createdNode = graph.GetAll()[ "Used" ];

        //check for the created class
        ClassCheck createdClass = Class(Priority.Low);
        //checking the check to set MatchedEntity to createdNode
        createdClass.Check(
            ctx,
            createdNode);

        //check for creating class, which creates the createdNode
        ClassCheck creatingClass = Class(
            Priority.Low,
            Creates(
                Priority.Low,
                createdClass));

        ICheckResult result = creatingClass.Check(
            ctx,
            creatingNode);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Creates_Check_Returns_Correct_False_Result()
    {
        //a syntax graph where the so called Used class does NOT create an instance of the so called Uses class
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        //the Uses class node from the syntax graph 
        INode creatingNode = graph.GetAll()[ "Uses" ];
        //the Used class node from the syntax graph 
        INode createdNode = graph.GetAll()[ "Used" ];

        //we check whether the createdNode creates an instance of the creatingNode. Something which is not the case

        //check for the created class
        ClassCheck createdClass = Class(Priority.Low);
        //checking the check to set MatchedEntity to creatingNode
        createdClass.Check(
            ctx,
            creatingNode);

        //check for the creating class creating the created creatingNode, which isnt the case
        ClassCheck creatingClass = Class(
            Priority.Low,
            Creates(
                Priority.Low,
                //the result of the createdClass, which is the creatingNode
                createdClass));

        ICheckResult result = creatingClass.Check(
            ctx,
            createdNode);

        return Verifier.Verify(result);
    }
}
