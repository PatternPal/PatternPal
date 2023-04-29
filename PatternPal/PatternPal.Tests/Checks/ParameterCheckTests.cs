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
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        RecognizerContext ctx = new() { Graph = graph };

        // Obtain the StringTestFunction method (0 parameters)
        IMethod stringNode = 
            graph.GetAll()["StringTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "StringTestFunction");

        // Obtain the IntTest method (1 parameter)
        IMethod intNode = 
            graph.GetAll()["IntTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "IntTestFunction");

        // TypeCheck of the StringTestFunction method (StringTest)
        TypeCheck typeStringNode = new TypeCheck(
            Priority.Low,
            OneOf<Func<List<INode>>, GetCurrentEntity>.FromT0(
                () => new List<INode> { stringNode }));
        ICheckResult test = typeStringNode.Check(ctx, stringNode);//TODO deze klopt, dus ik geef verkeerd door aan typecheck

        // TypeCheck of the IntTestFunction method (IntTest)
        TypeCheck typeIntNode = new TypeCheck(
            Priority.Low,
            OneOf<Func<List<INode>>, GetCurrentEntity>.FromT0(
                () => new List<INode> { intNode }));

        List<TypeCheck> collectiontest = new List<TypeCheck>
        {
            typeIntNode,
            typeStringNode

        };

        ParameterCheck usedParamCheck = new ParameterCheck(Priority.Low, collectiontest);

        MethodCheck method3 = new MethodCheck(
            Priority.Low,
            new List<ICheck>
            {
                usedParamCheck
            });

        // Parameter check with two typechecks of type "Uses" and "Used"
        //ParameterCheck usedParamCheck = 
        //    new ParameterCheck(Priority.Low, new List<TypeCheck>
        //    { 
        //        typeStringNode
        //    });

        ICheckResult res = method3.Check(ctx, intNode);
        return Verifier.Verify(res);

        // TODO Vraag aan refactor team hoe dit werkt
        // Eerste methodcheck { typecheck { Method met type "Uses"
        // Maak tweede methodcheck, refereer naar vorige.
        // Maak een parametercheck met de result van ??

    }

    [Test]
    public Task Parameter_Check_No_Parameters()
    {
        SyntaxGraph graph = EntityNodeUtils.CreateMethodWithParamaters();
        RecognizerContext ctx = new() { Graph = graph };

        // Obtain method with 0 parameters from syntax graph.
        IMethod stringNode =
            graph.GetAll()["StringTest"].GetMethods().FirstOrDefault(
                x => x.GetName() == "StringTestFunction");

        // Empty list of typechecks because check returns when checking parameters.
        ParameterCheck usedParamCheck =
            new ParameterCheck(Priority.Low, new List<TypeCheck> { });

        ICheckResult res = usedParamCheck.Check(ctx, stringNode);
        return Verifier.Verify(res);
    }
}
