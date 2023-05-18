#region

using PatternPal.SyntaxTree.Abstractions.Members;

#endregion

namespace PatternPal.Tests.Checks;

internal class PropertyCheckTests
{
    [Test]
    public void Property_Check_Accepts_Only_Properties()
    {
        IProperty propertyEntity = EntityNodeUtils.CreateProperty();
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        PropertyCheck propertyCheck = Property(Priority.Low);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        Assert.DoesNotThrow(
            () => propertyCheck.Check(
                ctx,
                propertyEntity));

        Assert.Throws< IncorrectNodeTypeException >(
            () => propertyCheck.Check(
                ctx,
                methodEntity));
    }

    [Test]
    public Task Property_Check_Returns_Correct_Result()
    {
        IProperty propertyEntity = EntityNodeUtils.CreateProperty();

        PropertyCheck propertyCheck = Property(Priority.Low);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = propertyCheck.Check(
            ctx,
            propertyEntity);

        return Verifier.Verify(result);
    }

    [Test]
    public void Property_Check_Handles_Incorrect_Nested_Check()
    {
        IProperty propertyEntity = EntityNodeUtils.CreateProperty();

        PropertyCheck propertyCheck = Property(
            Priority.Low,
            Class(Priority.Low));
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        Assert.Throws< InvalidSubCheckException >(
            () => propertyCheck.Check(
                ctx,
                propertyEntity));
    }

    [Test]
    public Task Property_Check_Nested_Modifier_Check()
    {
        IProperty propertyEntity = EntityNodeUtils.CreateProperty();

        PropertyCheck propertyCheck = Property(
            Priority.Low,
            Modifiers(
                Priority.Low,
                Modifier.Protected));
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = propertyCheck.Check(
            ctx,
            propertyEntity);

        return Verifier.Verify(result);
    }
}
