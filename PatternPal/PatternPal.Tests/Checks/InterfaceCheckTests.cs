#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class InterfaceCheckTests
{
    [Test]
    public void Interface_Check_Accepts_Only_Interfaces()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        IMethod methodEntity = EntityNodeUtils.CreateMethod();
        IInterface interfaceEntity = EntityNodeUtils.CreateInterface();

        InterfaceCheck interfaceCheck = Interface(Priority.Low);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        Assert.DoesNotThrow(
            () => interfaceCheck.Check(
                ctx,
                interfaceEntity));

        Assert.Throws< IncorrectNodeTypeException >(
            () => interfaceCheck.Check(
                ctx,
                methodEntity));

        Assert.Throws< IncorrectNodeTypeException >(
            () => interfaceCheck.Check(
                ctx,
                classEntity));
    }

    [Test]
    public Task Interface_Check_Returns_Correct_Result()
    {
        IInterface interfaceEntity = EntityNodeUtils.CreateInterface();

        InterfaceCheck interfaceCheck = Interface(Priority.Low);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = interfaceCheck.Check(
            ctx,
            interfaceEntity);

        return Verifier.Verify(result);
    }

    [Test]
    public void Interface_Check_Handles_Incorrect_Nested_Check()
    {
        IInterface interfaceEntity = EntityNodeUtils.CreateInterface();

        InterfaceCheck interfaceCheck = Interface(
            Priority.Low,
            Parameters(Priority.Low));
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        Assert.Throws< IncorrectNodeTypeException >(
            () => interfaceCheck.Check(
                ctx,
                interfaceEntity));
    }

    [Test]
    public Task Interface_Check_Nested_Modifier_Check()
    {
        IInterface interfaceEntity = EntityNodeUtils.CreateInterface();

        InterfaceCheck interfaceCheck = Interface(
            Priority.Low,
            Modifiers(
                Priority.Low,
                Modifier.Public));
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = interfaceCheck.Check(
            ctx,
            interfaceEntity);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Interface_Check_Nested_Method_Check()
    {
        IInterface interfaceEntity = EntityNodeUtils.CreateInterface();

        InterfaceCheck interfaceCheck = Interface(
            Priority.Low,
            Method(
                Priority.Low,
                Modifiers(
                    Priority.Mid,
                    Modifier.Public)));
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = interfaceCheck.Check(
            ctx,
            interfaceEntity);

        return Verifier.Verify(result);
    }
}
