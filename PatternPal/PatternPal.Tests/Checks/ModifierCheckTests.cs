#region

using SyntaxTree.Abstractions.Root;
using static PatternPal.Core.Builders.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class ModifierCheckTests
{
    [Test]
    public void Modifier_Check_Accepts_Only_ModifierChecks()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        INamespace namespaceEntity = EntityNodeUtils.CreateNamespace();

        RecognizerContext ctx = new();

        ModifierCheck modifierCheck = (ModifierCheck)Modifiers(Priority.Low).Build();

        Assert.DoesNotThrow(
            () => modifierCheck.Check(
                ctx,
                classEntity));

        Assert.Throws<IncorrectNodeTypeException>(
            () => modifierCheck.Check(
                ctx,
                namespaceEntity));
    }

    [Test]
    public Task Modifier_Check_Returns_Correct_True_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        ModifierCheck modifierCheck = (ModifierCheck)Modifiers(Priority.Low, Modifier.Public).Build();
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

        ModifierCheck modifierCheck = (ModifierCheck)Modifiers(Priority.Low, Modifier.Private).Build();
        RecognizerContext ctx = new();

        ICheckResult result = modifierCheck.Check(
            ctx,
            classEntity);

        return Verifier.Verify(result);
    }
}      
