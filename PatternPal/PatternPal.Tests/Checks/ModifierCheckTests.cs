#region

using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Root;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

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

        RecognizerContext ctx = new();

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
        RecognizerContext ctx = new();

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
        RecognizerContext ctx = new();

        ICheckResult result = modifierCheck.Check(
            ctx,
            classEntity);

        return Verifier.Verify(result);
    }
}
