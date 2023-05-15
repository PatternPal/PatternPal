#region

using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Abstractions.Members;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

internal class ConstructorCheckTests
{
    [Test]
    public void Constructor_Check_Accepts_Only_Constructors()
    {
        IConstructor constructorEntity = EntityNodeUtils.CreateConstructor(out _);
        IMethod methodEntity = EntityNodeUtils.CreateMethod();

        ConstructorCheck constructorCheck = Constructor(Priority.Low);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        Assert.DoesNotThrow(
            () => constructorCheck.Check(
                ctx,
                constructorEntity));

        Assert.Throws< IncorrectNodeTypeException >(
            () => constructorCheck.Check(
                ctx,
                methodEntity));
    }

    [Test]
    public Task Constructor_Check_Returns_Correct_Result()
    {
        IConstructor constructorEntity = EntityNodeUtils.CreateConstructor(out _);

        ConstructorCheck constructorCheck = Constructor(Priority.Low);
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = constructorCheck.Check(
            ctx,
            constructorEntity);

        return Verifier.Verify(result);
    }

    [Test]
    public void Constructor_Check_Handles_Incorrect_Nested_Check()
    {
        IConstructor constructorEntity = EntityNodeUtils.CreateConstructor(out SyntaxGraph graph);
        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph);

        ConstructorCheck constructorCheck = Constructor(
            Priority.Low,
            Class(Priority.Low));

        Assert.Throws< InvalidSubCheckException >(
            () => constructorCheck.Check(
                ctx,
                constructorEntity));
    }

    [Test]
    public Task Constructor_Check_Nested_Modifier_Check()
    {
        IConstructor constructorEntity = EntityNodeUtils.CreateConstructor(out _);

        ConstructorCheck constructorCheck = Constructor(
            Priority.Low,
            Modifiers(
                Priority.Low,
                Modifier.Public));
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = constructorCheck.Check(
            ctx,
            constructorEntity);

        return Verifier.Verify(result);
    }
}
