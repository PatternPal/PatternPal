#region

using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Abstractions.Members;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class ParameterCheckTests
{
    [Test]
    public Task Multiple_Same_parameters_Correct()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        // Obtain the StringTestFunction method (3 parameters)
        IMethod stringNode = Relations.GetMethodFromGraph(
            graph,
            "StringTest",
            "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode = Relations.GetMethodFromGraph(
            graph,
            "IntTest",
            "IntTestFunction");

        // Create same typecheck for two different parameters and one other type parameter
        TypeCheck typeIntNode1 = new TypeCheck(
            Priority.Low,
            OneOf< Func< List< INode > >, GetCurrentEntity >.FromT0(
                () => new List< INode >
                      {
                          ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType())
                      }));
        TypeCheck typeIntNode2 = typeIntNode1;
        TypeCheck typeStringNode = new TypeCheck(
            Priority.Low,
            OneOf< Func< List< INode > >, GetCurrentEntity >.FromT0(
                () => new List< INode >
                      {
                          ctx.Graph.Relations.GetEntityByName(stringNode.GetReturnType())
                      }));

        List< TypeCheck > collectiontest = new List< TypeCheck >
                                           {
                                               typeIntNode1,
                                               typeIntNode2,
                                               typeStringNode
                                           };

        ParameterCheck usedParamCheck = new ParameterCheck(
            Priority.Low,
            collectiontest);

        MethodCheck method3 = new MethodCheck(
            Priority.Low,
            new List< ICheck >
            {
                usedParamCheck
            });

        ICheckResult res = method3.Check(
            ctx,
            stringNode);
        return Verifier.Verify(res);
    }

    [Test]
    public Task Parameter_Check_Method_Different_Type()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        // Obtain the StringTestFunction method (3 parameters)
        IMethod stringNode = Relations.GetMethodFromGraph(
            graph,
            "StringTest",
            "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode = Relations.GetMethodFromGraph(
            graph,
            "IntTest",
            "IntTestFunction");

        // TypeCheck of the StringTestFunction method (return type is StringTest)
        TypeCheck typeIntNode = new TypeCheck(
            Priority.Low,
            OneOf< Func< List< INode > >, GetCurrentEntity >.FromT0(
                () => new List< INode >
                      {
                          ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType())
                      }));
        var test = ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType());

        List< TypeCheck > collectiontest = new List< TypeCheck >
                                           {
                                               typeIntNode
                                           };

        ParameterCheck usedParamCheck = new ParameterCheck(
            Priority.Low,
            collectiontest);

        MethodCheck method3 = new MethodCheck(
            Priority.Low,
            new List< ICheck >
            {
                usedParamCheck
            });

        // Incorrect parameter type check because typecheck has different type from parameter
        // of provided intNode.
        ICheckResult res = method3.Check(
            ctx,
            intNode);
        return Verifier.Verify(res);
    }

    [Test]
    public Task Parameter_Check_Method_Same_Type()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        // Obtain the StringTestFunction method (3 parameters)
        IMethod stringNode = Relations.GetMethodFromGraph(
            graph,
            "StringTest",
            "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode = Relations.GetMethodFromGraph(
            graph,
            "IntTest",
            "IntTestFunction");

        // TypeCheck of the StringTestFunction method (return type is StringTest)
        TypeCheck typeStringNode = new TypeCheck(
            Priority.Low,
            OneOf< Func< List< INode > >, GetCurrentEntity >.FromT0(
                () => new List< INode >
                      {
                          ctx.Graph.Relations.GetEntityByName(stringNode.GetReturnType())
                      }));

        List< TypeCheck > collectiontest = new List< TypeCheck >
                                           {
                                               typeStringNode
                                           };

        ParameterCheck usedParamCheck = new ParameterCheck(
            Priority.Low,
            collectiontest);

        MethodCheck method3 = new MethodCheck(
            Priority.Low,
            new List< ICheck >
            {
                usedParamCheck
            });

        ICheckResult res = method3.Check(
            ctx,
            intNode);
        return Verifier.Verify(res);
    }

    [Test]
    public Task Parameter_Check_No_Parameters()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateUsesRelation();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        // Obtain method with 0 parameters from syntax graph.
        IMethod stringNode = Relations.GetMethodFromGraph(
            graph,
            "Uses",
            "UsesFunction");

        // Empty list of typechecks because check returns when checking parameters.
        ParameterCheck usedParamCheck =
            new ParameterCheck(
                Priority.Low,
                new List< TypeCheck >
                {
                });

        ICheckResult res = usedParamCheck.Check(
            ctx,
            stringNode);
        return Verifier.Verify(res);
    }

    [Test]
    public Task Less_Parameters_Than_TypeChecks()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        // Obtain the StringTestFunction method (3 parameters)
        IMethod stringNode = Relations.GetMethodFromGraph(
            graph,
            "StringTest",
            "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode = Relations.GetMethodFromGraph(
            graph,
            "IntTest",
            "IntTestFunction");

        // Create two typechecks for a parameter check on a method with one parameter
        TypeCheck typeIntNode = new TypeCheck(
            Priority.Low,
            OneOf< Func< List< INode > >, GetCurrentEntity >.FromT0(
                () => new List< INode >
                      {
                          ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType())
                      }));
        TypeCheck typeStringNode = new TypeCheck(
            Priority.Low,
            OneOf< Func< List< INode > >, GetCurrentEntity >.FromT0(
                () => new List< INode >
                      {
                          ctx.Graph.Relations.GetEntityByName(stringNode.GetReturnType())
                      }));

        List< TypeCheck > collectiontest = new List< TypeCheck >
                                           {
                                               typeStringNode,
                                               typeIntNode
                                           };
        ParameterCheck usedParamCheck = new ParameterCheck(
            Priority.Low,
            collectiontest);
        MethodCheck method3 = new MethodCheck(
            Priority.Low,
            new List< ICheck >
            {
                usedParamCheck
            });

        ICheckResult res = method3.Check(
            ctx,
            intNode);
        return Verifier.Verify(res);
    }
}
