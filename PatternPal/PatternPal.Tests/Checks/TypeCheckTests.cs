using PatternPal.SyntaxTree.Abstractions.Entities;
using PatternPal.SyntaxTree.Abstractions.Members;

namespace PatternPal.Tests.Checks;

[TestFixture]
public class TypeCheckTests
{
    [Test]
    public Task Type_Check_Returns_Correct_Result()
    {
        IClass classEntity = EntityNodeUtils.CreateClass();

        TypeCheck typeCheck = new(
            Priority.Low,
            OneOf< Func< List< INode > >, GetCurrentEntity >.FromT0(
                () => new List< INode >
                      {
                          classEntity
                      }) );

        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

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
            OneOf< Func< List< INode > >, GetCurrentEntity >.FromT0(
                () => new List< INode >
                      {
                          classEntity
                      }) );

        IRecognizerContext ctx = RecognizerContext4Tests.Empty();

        ICheckResult result = typeCheck.Check(
            ctx,
            method);
        return Verifier.Verify(result);
    }
}
