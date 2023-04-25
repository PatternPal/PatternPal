#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class CheckHelperTests
{
    [Test]
    public void CheckAllChildrenCorrectTest()
    {
        // create the leaf nodes
        ICheckResult correctLeaf = new LeafCheckResult{ Correct = true, FeedbackMessage = "", Priority = Priority.Mid };
        ICheckResult incorrectLeaf = new LeafCheckResult { Correct = false, FeedbackMessage = "", Priority = Priority.Mid };
        

        // create the incorrect node
        List<ICheckResult> childrenIncorrectNode = new List<ICheckResult>{ correctLeaf, incorrectLeaf };
        ICheckResult nodeIncorrectResult = new NodeCheckResult{ChildrenCheckResults = childrenIncorrectNode, FeedbackMessage = "", Priority = Priority.Mid};
        
        // create the correct node
        List<ICheckResult> childrenCorrectNode = new List<ICheckResult> { correctLeaf, correctLeaf };
        ICheckResult nodeCorrectResult = new NodeCheckResult { ChildrenCheckResults = childrenCorrectNode, FeedbackMessage = "", Priority = Priority.Mid };

        // test the correct leaf node
        bool leafCorrect = PatternPal.Core.Checks.CheckHelper.CheckAllChildrenCorrect(correctLeaf);
        Assert.AreEqual(true, leafCorrect);
        
        // test the incorrect leaf node
        bool leafIncorrect = PatternPal.Core.Checks.CheckHelper.CheckAllChildrenCorrect(incorrectLeaf);
        Assert.AreEqual(false, leafIncorrect);


        // test the incorrect non-leaf node
        bool nodeInCorrect = PatternPal.Core.Checks.CheckHelper.CheckAllChildrenCorrect(nodeIncorrectResult);
        Assert.AreEqual(false, nodeInCorrect);

        // test the correct non-leaf node
        bool nodeCorrect = PatternPal.Core.Checks.CheckHelper.CheckAllChildrenCorrect(nodeCorrectResult);
        Assert.AreEqual(true, nodeCorrect);
    }

}
