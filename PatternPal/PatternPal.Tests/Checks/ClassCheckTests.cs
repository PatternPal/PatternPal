#region

using static PatternPal.Core.Builders.CheckBuilder;

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

        ClassCheck classCheck = (ClassCheck)Class().Build();
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
    public Task Class_Check_Returns_Correct_Results()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        ClassCheck classCheck = (ClassCheck)Class().Build();
        RecognizerContext ctx = new();

        ICheckResult result = classCheck.Check(
            ctx,
            classEntity);

        return Verifier.Verify(result);
    }
}
