#region

using static PatternPal.Core.Builders.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

internal class MethodCheckTests
{
    [Test]
    public void Method_Check_Accepts_Only_Methods()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        MethodCheck methodCheck = (MethodCheck)Method(Priority.Low).Build();
        RecognizerContext ctx = new();

        Assert.DoesNotThrow(
            () => methodCheck.Check(
                ctx,
                methodEntity));

        Assert.Throws<IncorrectNodeTypeException>(
            () => methodCheck.Check(
                ctx,
                classEntity));
    }

    [Test]
    public Task Method_Check_Returns_Correct_Result()
    {
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        MethodCheck methodCheck = (MethodCheck)Method(Priority.Low).Build();
        RecognizerContext ctx = new();

        ICheckResult result = methodCheck.Check(
            ctx,
            methodEntity);

        return Verifier.Verify(result);
    }

    [Test]
    public void Method_Check_Handles_Incorrect_Nested_Check()
    {
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        MethodCheck methodCheck = (MethodCheck)Method(Priority.Low, Class(Priority.Low)).Build();
        RecognizerContext ctx = new();

        Assert.Throws<InvalidSubCheckException>(
            () => methodCheck.Check(
                ctx,
                methodEntity));
    }

    [Test]
    public Task Method_Check_Nested_Modifier_Check()
    {
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        MethodCheck methodCheck = (MethodCheck)Method(Priority.Low, Modifiers(Priority.Low, Modifier.Internal)).Build();
        RecognizerContext ctx = new();

        ICheckResult result = methodCheck.Check(
            ctx,
            methodEntity);

        return Verifier.Verify(result);
    }
}
