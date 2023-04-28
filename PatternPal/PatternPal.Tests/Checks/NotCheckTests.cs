#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

internal class NotCheckTests
{
    [Test]
    public void Single_Modifier_Correct_Not_Check_Test()
    {
        // create classEntity and RecognizerContext
        IClass classEntity = EntityNodeUtils.CreateClass();
        RecognizerContext ctx = new();

        // create modifierCheck for private modifier
        ModifierCheck modifierCheck2 = new ModifierCheck(Priority.Low, new List<IModifier> { Modifier.Private });

        // create NotCheck for private modifier
        NotCheck notCheck2 = Not(Priority.Low, modifierCheck2);

        // create expected result for the private modifier
        NodeCheckResult expectedResult2 = new NodeCheckResult { Priority = Priority.Low, FeedbackMessage = "", ChildrenCheckResults = new List<ICheckResult>() };

        // get result of NotCheck
        NodeCheckResult result2 = (NodeCheckResult)notCheck2.Check(ctx, classEntity);

        Assert.AreEqual(expectedResult2.GetType(), result2.GetType());
        Assert.AreEqual(expectedResult2.Priority, result2.Priority);
        Assert.AreEqual(expectedResult2.ChildrenCheckResults.Count, result2.ChildrenCheckResults.Count);
    }

    [Test]
    public void Single_Modifier_Incorrect_Not_Check_Test()
    {
        // create classEntity and RecognizerContext
        IClass classEntity = EntityNodeUtils.CreateClass();
        RecognizerContext ctx = new();

        // create modifierCheck for the public modifier
        ModifierCheck modifierCheck1 = new ModifierCheck(Priority.Low, new List<IModifier> { Modifier.Public });

        // create NotCheck for public modifier
        NotCheck notCheck1 = Not(Priority.Low, modifierCheck1);

        // create expected result for public modifier
        ICheckResult leafModCheckResult1 =
            new LeafCheckResult { Priority = Priority.Low, FeedbackMessage = "", Correct = true };
        NodeCheckResult expectedResult1 = new NodeCheckResult { Priority = Priority.Low, FeedbackMessage = "", ChildrenCheckResults = new List<ICheckResult> { leafModCheckResult1 } };

        // get result of NotCheck
        NodeCheckResult result1 = (NodeCheckResult)notCheck1.Check(ctx, classEntity);

        Assert.AreEqual(expectedResult1.GetType(), result1.GetType());
        Assert.AreEqual(expectedResult1.Priority, result1.Priority);
        Assert.AreEqual(expectedResult1.ChildrenCheckResults.Count, result1.ChildrenCheckResults.Count);

    }

    [Test]
    public void Multiple_Modifiers_Not_Check_Test()
    {
        // create classEntity and RecognizerContext
        IClass classEntity = EntityNodeUtils.CreateClass();
        RecognizerContext ctx = new();

        // create modifierCheck for multiple modifiers
        ModifierCheck modifierCheck = new ModifierCheck(Priority.Low, new List<IModifier> { Modifier.Public, Modifier.Private });

        // create NotCheck for multiple modifiers
        NotCheck notCheck = Not(Priority.Low, modifierCheck);

        // create expected result for multiple modifiers
        ICheckResult leafModCheckResult =
            new LeafCheckResult { Priority = Priority.Low, FeedbackMessage = "", Correct = false };
        NodeCheckResult expectedResult = new NodeCheckResult { Priority = Priority.Low, FeedbackMessage = "", ChildrenCheckResults = new List<ICheckResult> () };

        // get result of NotCheck
        NodeCheckResult result = (NodeCheckResult)notCheck.Check(ctx, classEntity);

        Assert.AreEqual(expectedResult.GetType(), result.GetType());
        Assert.AreEqual(expectedResult.Priority, result.Priority);
        Assert.AreEqual(expectedResult.ChildrenCheckResults.Count, result.ChildrenCheckResults.Count);

    }

}

