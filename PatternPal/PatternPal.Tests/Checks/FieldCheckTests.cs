#region

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Tests.Checks
{
    public class FieldCheckTests
    {
        [Test]
        public void Field_Check_Accepts_Only_Fields()
        {
            IField fieldEntity = EntityNodeUtils.CreateField();
            IClass classEntity = EntityNodeUtils.CreateClass();

            FieldCheck fieldCheck = Field(Priority.Low);
            IRecognizerContext ctx = RecognizerContext4Tests.Empty();

            Assert.DoesNotThrow(
                () => fieldCheck.Check(
                    ctx,
                    fieldEntity));

            Assert.Throws< IncorrectNodeTypeException >(
                () => fieldCheck.Check(
                    ctx,
                    classEntity));
        }

        [Test]
        public Task Field_Check_Returns_Correct_Result()
        {
            IField fieldEntity = EntityNodeUtils.CreateField();

            FieldCheck fieldCheck = Field(Priority.Low);
            IRecognizerContext ctx = RecognizerContext4Tests.Empty();

            ICheckResult result = fieldCheck.Check(
                ctx,
                fieldEntity);

            return Verifier.Verify(result);
        }

        [Test]
        public void Field_Check_Handles_Incorrect_Nested_Check()
        {
            IField fieldEntity = EntityNodeUtils.CreateField();

            FieldCheck fieldCheck = Field(
                Priority.Low,
                Parameters(Priority.Low));
            IRecognizerContext ctx = RecognizerContext4Tests.Empty();

            Assert.Throws< IncorrectNodeTypeException >(
                () => fieldCheck.Check(
                    ctx,
                    fieldEntity));
        }

        [Test]
        public Task Field_Check_Nested_Modifier_Check()
        {
            IField fieldEntity = EntityNodeUtils.CreateField();

            FieldCheck fieldCheck = Field(
                Priority.Low,
                Modifiers(
                    Priority.Low,
                    Modifier.Public));
            IRecognizerContext ctx = RecognizerContext4Tests.Empty();

            ICheckResult result = fieldCheck.Check(
                ctx,
                fieldEntity);

            return Verifier.Verify(result);
        }
    }
}
