
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
                sr.OnlyPrivateConstructor(out _);

            // Needs to be in a class in order to be tested
            ClassCheck classOnlyPrivateConstructorCheck = Class(
                Priority.Low,
                onlyPrivateConstructorCheck);

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
        public Task HasCorrectMethodModifiersTest()
        {
            // Create a graph of a singleton and check if its methods have the correct modifiers
            SyntaxGraph graph = EntityNodeUtils.CreateCorrectSingleton();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            // Method to check
            ICheck[] hasStaticPublicInternalMethod =
                sr.HasStaticPublicInternalMethod();

            // Put in a class, otherwise cannot be tested
            ClassCheck classHasStaticPublicInternalMethod = Class(
                Priority.Low,
                Method(
                    Priority.Mid,
                    hasStaticPublicInternalMethod
                )
            );

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
        [Test]
        public Task HasWrongMethodModifiersTest()
        {
            // Create two graphs of two classes where one of the modifiers of methods are missing
            SyntaxGraph graph = EntityNodeUtils.CreateSingletonWrongMethodModifiers();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            // Method to check
            ICheck[] hasStaticPublicInternalMethod =
                sr.HasStaticPublicInternalMethod();

            // Put in a class, otherwise cannot be tested
            ClassCheck classHasStaticPublicInternalMethod = Class(
                Priority.Low,
                Method(
                    Priority.Mid,
                    hasStaticPublicInternalMethod
                )
            );

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

        [Test]
        public Task CallsPrivateConstructorTest()
        {
            // Create a graph with a correct private constructor call
            SyntaxGraph graph = EntityNodeUtils.CreateCorrectSingleton();
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

        [Test]
        public Task DoesNotCallPrivateConstructorTest()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first and the last two
            // adheres to all requirements and the second and third are missing one specific modifier
            SyntaxGraph graph = EntityNodeUtils.CreateSingletonNoConstructorCall();
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


        [Test]
        public Task DoesReturnPrivateFieldTest()
        {
            // Create a graph of a correct singleton which returns its private field
            SyntaxGraph graph = EntityNodeUtils.CreateCorrectSingleton();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            FieldCheck staticPrivateFieldOfTypeClass =
                sr.StaticPrivateFieldOfTypeClass();

            // Method to check
            ICheck[] returnsPrivateField =
                sr.ReturnsPrivateField(staticPrivateFieldOfTypeClass);

            // Put in a class, otherwise cannot be tested
            ClassCheck classReturnsPrivateField = Class(
                Priority.Low,
                staticPrivateFieldOfTypeClass,
                Method(
                    Priority.Mid,
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

        [Test]
        public Task DoesNotReturnPrivateFieldTest()
        {
            // Create a graph of an incorrect singleton which does not return its private field
            SyntaxGraph graph = EntityNodeUtils.CreateSingletonNoFieldUsage();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            FieldCheck staticPrivateFieldOfTypeClass =
                sr.StaticPrivateFieldOfTypeClass();

            // Method to check
            ICheck[] returnsPrivateField =
                sr.ReturnsPrivateField(staticPrivateFieldOfTypeClass);

            // Put in a class, otherwise cannot be tested
            ClassCheck classReturnsPrivateField = Class(
                Priority.Low,
                staticPrivateFieldOfTypeClass,
                Method(
                    Priority.Mid,
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

        [Test]
        public Task StaticMethodActsAsConstructorTestCorrect()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first adheres to all requirements
            // and the last four are all missing one specific requirement

            SyntaxGraph graph = EntityNodeUtils.CreateCorrectSingleton();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            sr.OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck);

            // Step 3 of Singleton StepByStep
            ClassCheck hasMethodActsAsConstructor =
                sr.CheckMethodAcsAsConstructorBehaviour(
                    privateConstructorCheck,
                    sr.StaticPrivateFieldOfTypeClass(),
                    out _);

            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res =  hasMethodActsAsConstructor.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        [Test]
        public Task StaticMethodActsAsConstructorTestWrongModifiers()
        {
            // Create a graph of a singleton class with wrong method modifiers

            SyntaxGraph graph = EntityNodeUtils.CreateSingletonWrongMethodModifiers();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            sr.OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck);

            // Step 3 of Singleton StepByStep
            ClassCheck hasMethodActsAsConstructor =
                sr.CheckMethodAcsAsConstructorBehaviour(
                    privateConstructorCheck,
                    sr.StaticPrivateFieldOfTypeClass(),
                    out _);

            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = hasMethodActsAsConstructor.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        [Test]
        public Task StaticMethodActsAsConstructorTestNoConstructorCall()
        {
            // Create a graph of a singleton class with no call to the constructor

            SyntaxGraph graph = EntityNodeUtils.CreateSingletonNoConstructorCall();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            sr.OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck);

            // Step 3 of Singleton StepByStep
            ClassCheck hasMethodActsAsConstructor =
                sr.CheckMethodAcsAsConstructorBehaviour(
                    privateConstructorCheck,
                    sr.StaticPrivateFieldOfTypeClass(),
                    out _);

            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = hasMethodActsAsConstructor.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        [Test]
        public Task StaticMethodActsAsConstructorTestNoFieldUsage()
        {
            // Create a graph of a singleton class with no usage of the private field

            SyntaxGraph graph = EntityNodeUtils.CreateSingletonNoFieldUsage();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            sr.OnlyPrivateConstructor(out ConstructorCheck privateConstructorCheck);

            // Step 3 of Singleton StepByStep
            ClassCheck hasMethodActsAsConstructor =
                sr.CheckMethodAcsAsConstructorBehaviour(
                    privateConstructorCheck,
                    sr.StaticPrivateFieldOfTypeClass(),
                    out _);

            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = hasMethodActsAsConstructor.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        [Test]
        public Task ClientDoesCallMethodActsAsConstructorTest()
        {
            // Create a graph of a singleton class where the client uses the singleton
            SyntaxGraph graph = EntityNodeUtils.CreateCorrectClientSingleton();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            //Method which the client should call
            MethodCheck publicInternalMethod = Method(
                Priority.Mid,
                sr.HasStaticPublicInternalMethod()
            );

            // Client call check
            ClassCheck clientCallsMethodActsAsConstructor =
                sr.ClientCallsMethodActsAsConstructor(
                    publicInternalMethod);

            ICheck checkBothClasses = All(
                Priority.Low,
                Class(
                    Priority.High,
                    publicInternalMethod
                ),
                clientCallsMethodActsAsConstructor
            );

            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = checkBothClasses.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

        [Test]
        public Task ClientDoesNotCallMethodActsAsConstructorTest()
        {
            // Create a graph of a singleton class where the client does not use the singleton
            SyntaxGraph graph = EntityNodeUtils.CreateWrongClientSingleton();
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);

            SingletonRecognizer sr = new();
            List<ICheckResult> results = new();

            //Method which the client should call
            MethodCheck publicInternalMethod = Method(
                Priority.Mid,
                sr.HasStaticPublicInternalMethod()
            );

            // Client call check
            ClassCheck clientCallsMethodActsAsConstructor =
                sr.ClientCallsMethodActsAsConstructor(
                    publicInternalMethod);

            ICheck checkBothClasses = All(
                Priority.Low,
                Class(
                    Priority.High,
                    publicInternalMethod
                ),
                clientCallsMethodActsAsConstructor
            );

            Dictionary<string, IEntity> entireTree = graph.GetAll();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = checkBothClasses.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

    }
}
