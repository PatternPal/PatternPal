#region

using PatternPal.SyntaxTree.Abstractions.Root;

#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class ModifierCheckTests
{
    [Test]
    public void Modifier_Check_Accepts_Only_ModifierChecks()
    {
        //a class can have modifiers
        IClass classEntity = EntityNodeUtils.CreateClass();
        //a namespace cannot have modifiers
        INamespace namespaceEntity = EntityNodeUtils.CreateNamespace();

        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ModifierCheck modifierCheck = Modifiers(Priority.Low);

        Assert.DoesNotThrow(
            () => modifierCheck.Check(
                ctx,
                classEntity));

        Assert.Throws< IncorrectNodeTypeException >(
            () => modifierCheck.Check(
                ctx,
                namespaceEntity));
    }

    [Test]
    public Task Modifier_Check_Returns_Correct_True_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        ModifierCheck modifierCheck = Modifiers(
            Priority.Low,
            Modifier.Public);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = modifierCheck.Check(
            ctx,
            classEntity);

        return Verifier.Verify(result);
    }

    [Test]
    public Task Modifier_Check_Returns_Correct_False_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        ModifierCheck modifierCheck = Modifiers(
            Priority.Low,
            Modifier.Private);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = modifierCheck.Check(
            ctx,
            classEntity);

        return Verifier.Verify(result);
    }
}
