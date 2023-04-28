#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class ClassCheckTests
{
    [Test]
    public void Class_Check_Accepts_Only_Classes()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        ClassCheck classCheck = Class(Priority.Low);
        RecognizerContext ctx = new();

        Assert.DoesNotThrow(
            () => classCheck.Check(
                ctx,
                classEntity));

        Assert.Throws< IncorrectNodeTypeException >(
            () => classCheck.Check(
                ctx,
                methodEntity));
    }

    [Test]
    public Task Class_Check_Returns_Correct_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        ClassCheck classCheck = Class(Priority.Low);
        RecognizerContext ctx = new();

        ICheckResult result = classCheck.Check(
            ctx,
            classEntity);

        return Verifier.Verify(result);
    }

    [Test]
    public void Class_Check_Handles_Incorrect_Nested_Check()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        ClassCheck classCheck = Class(
            Priority.Low,
            Parameters(Priority.Low));
        RecognizerContext ctx = new();

        Assert.Throws< InvalidSubCheckException >(
            () => classCheck.Check(
                ctx,
                classEntity));
    }

    [Test]
    public Task Class_Check_Nested_Modifier_Check()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        ClassCheck classCheck = Class(
            Priority.Low,
            Modifiers(
                Priority.Low,
                Modifier.Public));
        RecognizerContext ctx = new();

        ICheckResult result = classCheck.Check(
            ctx,
            classEntity);

        return Verifier.Verify(result);
    }
}
