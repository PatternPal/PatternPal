#region

using PatternPal.SyntaxTree.Abstractions;
using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

internal class NotCheckTests
{
    [Test]
    public void Not_Check_Should_Not_Be_Called_Directly()
    {
        NotCheck notCheck = Not(
            Priority.Low,
            Modifiers(
                Priority.Low,
                Modifier.Abstract));

        IClass classEntity = EntityNodeUtils.CreateClass();
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        Assert.Throws< UnreachableException >(
            () => notCheck.Check(
                ctx,
                classEntity));
    }

    [Test]
    public Task Single_Modifier_Correct_NotCheck_Test()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        NodeCheck< INode > checkCollection = Any(
            Priority.Low,
            Not(
                Priority.Low,
                Modifiers(
                    Priority.Low,
                    Modifier.Private)));

        ICheckResult result = checkCollection.Check(
            ctx,
            classEntity);
        return Verifier.Verify(result);
    }

    [Test]
    public Task Single_Modifier_Incorrect_NotCheck_Test()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();
        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        NodeCheck< INode > checkCollection = Any(
            Priority.Low,
            Not(
                Priority.Low,
                Modifiers(
                    Priority.Low,
                    Modifier.Public)));

        ICheckResult result = checkCollection.Check(
            ctx,
            classEntity);
        return Verifier.Verify(result);
    }
}
