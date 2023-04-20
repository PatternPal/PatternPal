namespace PatternPal.Tests.Checks;

[TestFixture]
public class TypeCheckTests
{
    [Test]
    public void Type_Check_Cannot_Have_No_Functors()
    {
        TypeCheck typeCheck = new(
            Priority.Low,
            null,
            null );

        IClass classEntity = EntityNodeUtils.CreateClass();
        RecognizerContext ctx = new();

        Assert.Throws< ArgumentException >(
            () => typeCheck.Check(
                ctx,
                classEntity));
    }

    [Test]
    public void Type_Check_Cannot_Have_Two_Functors()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        TypeCheck typeCheck = new(
            Priority.Low,
            () => classEntity,
            ctx => ctx.CurrentEntity );

        RecognizerContext ctx = new();

        Assert.Throws< ArgumentException >(
            () => typeCheck.Check(
                ctx,
                classEntity));
    }

    [Test]
    public void Type_Check_Has_One_Functor()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        TypeCheck typeCheck1 = new(
            Priority.Low,
            () => classEntity,
            null );

        TypeCheck typeCheck2 = new(
            Priority.Low,
            null,
            ctx => ctx.CurrentEntity );

        RecognizerContext ctx = new();

        Assert.DoesNotThrow(
            () => typeCheck1.Check(
                ctx,
                classEntity));

        Assert.DoesNotThrow(
            () => typeCheck2.Check(
                ctx,
                classEntity));
    }

    [Test]
    public Task Type_Check_Returns_Correct_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        TypeCheck typeCheck = new(
            Priority.Low,
            () => classEntity,
            null );

        RecognizerContext ctx = new();

        ICheckResult result = typeCheck.Check(
            ctx,
            classEntity);
        return Verifier.Verify(result);
    }

    [Test]
    public Task Type_Check_Returns_Incorrect_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        IMethod method = EntityNodeUtils.CreateMethod();

        TypeCheck typeCheck = new(
            Priority.Low,
            () => classEntity,
            null );

        RecognizerContext ctx = new();

        ICheckResult result = typeCheck.Check(
            ctx,
            method);
        return Verifier.Verify(result);
    }
}
