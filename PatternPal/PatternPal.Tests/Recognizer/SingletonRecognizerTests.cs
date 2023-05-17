
using PatternPal.Core.Recognizers;
using PatternPal.SyntaxTree;
using PatternPal.SyntaxTree.Abstractions.Entities;
using static PatternPal.Core.Checks.CheckBuilder;

namespace PatternPal.Tests.Recognizer
{
    [TestFixture]
    internal class SingletonRecognizerTests
    {
        [Test]
        public Task OnlyPrivateConstructorTest()
        {
            // Create a graph of 5 classes with 4 different constructors with private, protected,
            // public and internal modifiers, and one with both private and internal
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleConstructors();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            // Step 1 of Singleton StepByStep
            ICheck onlyPrivateConstructorCheck =
                sr.OnlyPrivateConstructor(out ConstructorCheck test);

            // Needs to be in a class in order to be tested
            ClassCheck classOnlyPrivateConstructorCheck = Class(
                Priority.Low,
                onlyPrivateConstructorCheck);

            // Obtain all classes of the graph
            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = classOnlyPrivateConstructorCheck.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        [Test]
        public Task StaticPrivateFieldOfTypeClassTest()
        {
            // Create a graph of 4 classes with 4 different fields where the first adheres to all 3 requirements
            // and the last three are all missing one specific requirement
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleFields();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            // Step 2 of Singleton StepByStep
            ICheck hasCorrectFieldCheck =
                sr.StaticPrivateFieldOfTypeClass();

            // Put in a class, otherwise cannot be tested
            ClassCheck classHasCorrectFieldCheck = Class(
                Priority.Low,
                hasCorrectFieldCheck);

            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = classHasCorrectFieldCheck.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        [Test]
        public Task HasStaticPublicInternalMethodTest()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first and the last two
            // adheres to all requirements and the second and third are missing one specific modifier
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleSingletons();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            // Method to check
            ICheck hasStaticPublicInternalMethod =
                sr.HasStaticPublicInternalMethod();

            // Put in a class, otherwise cannot be tested
            ClassCheck classHasStaticPublicInternalMethod = Class(
                Priority.Low,
                hasStaticPublicInternalMethod);

            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = classHasStaticPublicInternalMethod.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        //TODO: Find out why the uses relation always returns true
        //TODO: Constructor.Result always returns zero. Find out why
        [Test]
        public Task CallsPrivateConstructorTest()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first and the last two
            // adheres to all requirements and the second and third are missing one specific modifier
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleSingletons();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            sr.OnlyPrivateConstructor(out ConstructorCheck constructor);

            // Method to check
            ICheck callsPrivateConstructor =
                sr.CallsPrivateConstructor(constructor);

            // Put in a class, otherwise cannot be tested
            ClassCheck classCallsPrivateConstructor = Class(
                Priority.Low,
                constructor,
                Method(
                    Priority.Low,
                    callsPrivateConstructor
                )
            );
            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = classCallsPrivateConstructor.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        //[Test]
        public Task CheckSingletonD1Test()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first and the last two
            // adheres to all requirements and the second and third are missing one specific modifier
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleSingletons();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            MethodCheck hasStaticPublicInternalMethod = sr.HasStaticPublicInternalMethod();

            sr.OnlyPrivateConstructor(out ConstructorCheck constructor);
            ICheck callsPrivateConstructor =
                sr.CallsPrivateConstructor(constructor);

            // Method to check
            MethodCheck checkSingletonD1 =
                sr.CheckSingletonD1(hasStaticPublicInternalMethod, callsPrivateConstructor);

            // Put in a class, otherwise cannot be tested
            ClassCheck classCheckSingletonD1 = Class(
                Priority.Low,
                Method(
                    Priority.Low,
                    checkSingletonD1
                )
            );
            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = classCheckSingletonD1.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        //[Test]
        public Task ReturnsPrivateFieldTest()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first and the last two
            // adheres to all requirements and the second and third are missing one specific modifier
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleSingletons();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            FieldCheck staticPrivateFieldOfTypeClass =
                sr.StaticPrivateFieldOfTypeClass();

            // Method to check
            ICheck returnsPrivateField =
                sr.ReturnsPrivateField(staticPrivateFieldOfTypeClass);

            // Put in a class, otherwise cannot be tested
            ClassCheck classReturnsPrivateField = Class(
                Priority.Low,
                Method(
                    Priority.Low,
                    returnsPrivateField
                )
            );
            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = classReturnsPrivateField.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        //[Test]
        public Task CheckSingletonD2Test()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first and the last two
            // adheres to all requirements and the second and third are missing one specific modifier
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleSingletons();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            MethodCheck hasStaticPublicInternalMethod = sr.HasStaticPublicInternalMethod();
            ICheck checkInstanceConstructor = 
                sr.ReturnsPrivateField(
                    sr.StaticPrivateFieldOfTypeClass());

            // Method to check
            MethodCheck checkSingletonD2 =
                sr.CheckSingletonD2(hasStaticPublicInternalMethod, checkInstanceConstructor);

            // Put in a class, otherwise cannot be tested
            ClassCheck classCheckSingletonD2 = Class(
                Priority.Low,
                Method(
                    Priority.Low,
                    checkSingletonD2
                )
            );
            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = classCheckSingletonD2.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        //[Test]
        public Task StaticMethodActsAsConstructorTest()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first adheres to all requirements
            // and the last four are all missing one specific requirement

            SyntaxGraph graph = EntityNodeUtils.CreateMultipleSingletons();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            sr.OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck);

            // Step 3 of Singleton StepByStep
            ICheck hasMethodAcsAsConstructor =
                sr.CheckMethodAcsAsConstructorBehaviour(
                    privateConstructorCheck,
                    sr.StaticPrivateFieldOfTypeClass(),
                    out _);

            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = hasMethodAcsAsConstructor.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        //[Test]
        public Task ClientCallsMethodActsAsConstructorTest()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first and the last two
            // adheres to all requirements and the second and third are missing one specific modifier
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleSingletons();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            sr.OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck);

            // Step 3 of Singleton StepByStep
            ICheck hasMethodAcsAsConstructor =
                sr.CheckMethodAcsAsConstructorBehaviour(
                    privateConstructorCheck,
                    sr.StaticPrivateFieldOfTypeClass(),
                    out MethodCheck hasStaticPublicInternalMethod);

            // Method to check
            ICheck clientCallsMethodActsAsConstructor =
                sr.ClientCallsMethodActsAsConstructor(hasStaticPublicInternalMethod);

            // Put in a class, otherwise cannot be tested
            ClassCheck classClientCallsMethodActsAsConstructor = Class(
                Priority.Low,
                Method(
                    Priority.Low,
                    clientCallsMethodActsAsConstructor
                )
            );
            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = classClientCallsMethodActsAsConstructor.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

    }
}
