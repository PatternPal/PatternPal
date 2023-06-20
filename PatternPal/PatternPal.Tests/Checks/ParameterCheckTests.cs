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
        IMethod stringNode = EntityNodeUtils.GetMemberFromGraph< IMethod >(
            graph,
            "StringTest",
            "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode = EntityNodeUtils.GetMemberFromGraph< IMethod >(
            graph,
            "IntTest",
            "IntTestFunction");

        TestCheck intNodeCheck = new( ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType()) );
        TestCheck stringNodeCheck = new( ctx.Graph.Relations.GetEntityByName(stringNode.GetReturnType()) );

        // Create same typecheck for two different parameters and one other type parameter
        TypeCheck typeIntNode1 = new(
            Priority.Low,
            null,
            intNodeCheck );

        TypeCheck typeIntNode2 = typeIntNode1;
        TypeCheck typeStringNode = new(
            Priority.Low,
            null,
            stringNodeCheck );

        List< TypeCheck > collectiontest = new()
                                           {
                                               typeIntNode1,
                                               typeIntNode2,
                                               typeStringNode
                                           };

        ParameterCheck usedParamCheck = new(
            Priority.Low,
            null,
            collectiontest );

        MethodCheck method3 = new(
            Priority.Low,
            null,
            new List< ICheck >
            {
                usedParamCheck
            } );

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

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode = EntityNodeUtils.GetMemberFromGraph< IMethod >(
            graph,
            "IntTest",
            "IntTestFunction");

        TestCheck intNodeCheck = new( ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType()) );

        // TypeCheck of the StringTestFunction method (return type is StringTest)
        TypeCheck typeIntNode = new(
            Priority.Low,
            null,
            intNodeCheck );

        List< TypeCheck > collectiontest = new()
                                           {
                                               typeIntNode
                                           };

        ParameterCheck usedParamCheck = new(
            Priority.Low,
            null,
            collectiontest );

        MethodCheck method3 = new(
            Priority.Low,
            null,
            new List< ICheck >
            {
                usedParamCheck
            } );

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
        IMethod stringNode = EntityNodeUtils.GetMemberFromGraph< IMethod >(
            graph,
            "StringTest",
            "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode = EntityNodeUtils.GetMemberFromGraph< IMethod >(
            graph,
            "IntTest",
            "IntTestFunction");

        TestCheck stringNodeCheck = new( ctx.Graph.Relations.GetEntityByName(stringNode.GetReturnType()) );

        // TypeCheck of the StringTestFunction method (return type is StringTest)
        TypeCheck typeStringNode = new(
            Priority.Low,
            null,
            stringNodeCheck );

        List< TypeCheck > collectiontest = new()
                                           {
                                               typeStringNode
                                           };

        ParameterCheck usedParamCheck = new(
            Priority.Low,
            null,
            collectiontest );

        MethodCheck method3 = new(
            Priority.Low,
            null,
            new List< ICheck >
            {
                usedParamCheck
            } );

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

        IMethod stringNode = EntityNodeUtils.GetMemberFromGraph< IMethod >(
            graph,
            "Uses",
            "UsesFunction");

        // Empty list of typechecks because check returns when checking parameters.
        ParameterCheck usedParamCheck =
            new ParameterCheck(
                Priority.Low,
                null,
                new List< TypeCheck >
                {
                });

        ICheckResult res = usedParamCheck.Check(
            ctx,
            stringNode);
        return Verifier.Verify(res);
    }

    [Test]
    public Task Parameter_Check_No_Type_Checks()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        IMethod intNode = EntityNodeUtils.GetMemberFromGraph< IMethod >(
            graph,
            "IntTest",
            "IntTestFunction");

        ParameterCheck usedParamCheck = Parameters(Priority.High);

        ICheckResult res = usedParamCheck.Check(
            ctx,
            intNode);
        return Verifier.Verify(res);
    }

    [Test]
    public Task Less_Parameters_Than_TypeChecks()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        // Obtain the StringTestFunction method (3 parameters)
        IMethod stringNode = EntityNodeUtils.GetMemberFromGraph< IMethod >(
            graph,
            "StringTest",
            "StringTestFunction");

        // Obtain the IntTest method (1 StringTest parameter)
        IMethod intNode = EntityNodeUtils.GetMemberFromGraph< IMethod >(
            graph,
            "IntTest",
            "IntTestFunction");

        TestCheck intNodeCheck = new( ctx.Graph.Relations.GetEntityByName(intNode.GetReturnType()) );
        TestCheck stringNodeCheck = new( ctx.Graph.Relations.GetEntityByName(stringNode.GetReturnType()) );

        // Create two typechecks for a parameter check on a method with one parameter
        TypeCheck typeIntNode = new(
            Priority.Low,
            null,
            intNodeCheck );
        TypeCheck typeStringNode = new(
            Priority.Low,
            null,
            stringNodeCheck );

        List< TypeCheck > collectiontest = new()
                                           {
                                               typeStringNode,
                                               typeIntNode
                                           };

        ParameterCheck usedParamCheck = new(
            Priority.Low,
            null,
            collectiontest );
        MethodCheck method3 = new(
            Priority.Low,
            null,
            new List< ICheck >
            {
                usedParamCheck
            } );

        ICheckResult res = method3.Check(
            ctx,
            intNode);
        return Verifier.Verify(res);
    }
}

file class TestCheck : ICheck
{
    public Priority Priority { get; }
    public string ? Requirement { get; }
    public Func< List< INode > > Result { get; }
    public int DependencyCount { get; }
    public ICheck ? ParentCheck { get; set; }
    public Score PerfectScore { get; }

    public ICheckResult Check(
        IRecognizerContext ctx,
        INode node)
    {
        throw new NotImplementedException();
    }

    internal TestCheck(
        IEntity entity)
    {
        Result = () => new List< INode >
                       {
                           entity
                       };
    }
}
