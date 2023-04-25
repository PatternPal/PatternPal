#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks;

[TestFixture]
public class CheckCollectionTests
{
    [Test]
    public Task Check_Collection_Returns_Correct_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        CheckCollection checkCollection = Any(
            Priority.Low,
            Class(Priority.Low));
        RecognizerContext ctx = new();

        ICheckResult result = checkCollection.Check(
            ctx,
            classEntity);

        return Verifier.Verify(result);
    }
}
