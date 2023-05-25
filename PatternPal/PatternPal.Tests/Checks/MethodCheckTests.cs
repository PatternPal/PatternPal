#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

internal class MethodCheckTests
{
    [Test]
    public void Method_Check_Accepts_Only_Methods()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        MethodCheck methodCheck = Method(Priority.Low);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        Assert.DoesNotThrow(
            () => methodCheck.Check(
                ctx,
                methodEntity));

        Assert.Throws< IncorrectNodeTypeException >(
            () => methodCheck.Check(
                ctx,
                classEntity));
    }

    [Test]
    public Task Method_Check_Returns_Correct_Result()
    {
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        MethodCheck methodCheck = Method(Priority.Low);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = methodCheck.Check(
            ctx,
            methodEntity);

        return Verifier.Verify(result);
    }

    [Test]
    public void Method_Check_Handles_Incorrect_Nested_Check()
    {
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        MethodCheck methodCheck = Method(
            Priority.Low,
            Class(Priority.Low));
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        Assert.Throws< InvalidSubCheckException >(
            () => methodCheck.Check(
                ctx,
                methodEntity));
    }

    [Test]
    public Task Method_Check_Nested_Modifier_Check()
    {
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        MethodCheck methodCheck = Method(
            Priority.Low,
            Modifiers(
                Priority.Low,
                Modifier.Internal));
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = methodCheck.Check(
            ctx,
            methodEntity);

        return Verifier.Verify(result);
    }
}
