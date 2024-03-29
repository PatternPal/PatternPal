﻿namespace PatternPal.Tests.Recognizer
{
    [TestFixture]
    internal class SingletonRecognizerTests
    {
        [Test]
        public Task OnlyPrivateProtectedConstructorTest()
        {
            // Create a graph of 5 classes with 4 different constructors with private, protected,
            // public and internal modifiers, and one with both private and internal
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleConstructors();
            SingletonRecognizer sr = new();

            // Step 1 of Singleton StepByStep
            ICheck onlyPrivateProtectedConstructorCheck =
                sr.OnlyPrivateProtectedConstructor(out _);

            // Needs to be in a class in order to be tested
            ClassCheck classOnlyPrivateProtectedConstructorCheck = Class(
                Priority.Low,
                onlyPrivateProtectedConstructorCheck);

            return TestICheck(classOnlyPrivateProtectedConstructorCheck, graph);
        }

        [Test]
        public Task StaticPrivateFieldOfTypeClassTest()
        {
            // Create a graph of 4 classes with 4 different fields where the first adheres to all 3 requirements
            // and the last three are all missing one specific requirement
            SyntaxGraph graph = EntityNodeUtils.CreateMultipleFields();
            SingletonRecognizer sr = new();

            // Step 2 of Singleton StepByStep
            ICheck hasCorrectFieldCheck =
                sr.StaticPrivateFieldOfTypeClass();

            // Put in a class, otherwise cannot be tested
            ClassCheck classHasCorrectFieldCheck = Class(
                Priority.Low,
                hasCorrectFieldCheck);

            return TestICheck(classHasCorrectFieldCheck, graph);
        }

        [Test]
        public Task HasCorrectMethodModifiersTest()
        {
            // Create a graph of a singleton and check if its methods have the correct modifiers
            SyntaxGraph graph = EntityNodeUtils.CreateCorrectSingleton();
            SingletonRecognizer sr = new();

            // Method to check
            ICheck[] hasStaticPublicInternalMethod =
                sr.IsStaticPublicOrInternal();

            // Put in a class, otherwise cannot be tested
            ClassCheck classIsStaticPublicOrInternal = Class(
                Priority.Low,
                Method(
                    Priority.Mid,
                    hasStaticPublicInternalMethod
                )
            );

            return TestICheck(classIsStaticPublicOrInternal, graph);
        }
        [Test]
        public Task HasWrongMethodModifiersTest()
        {
            // Create two graphs of two classes where one of the modifiers of methods are missing
            SyntaxGraph graph = EntityNodeUtils.CreateSingletonWrongMethodModifiers();
            SingletonRecognizer sr = new();

            // Method to check
            ICheck[] hasStaticPublicInternalMethod =
                sr.IsStaticPublicOrInternal();

            // Put in a class, otherwise cannot be tested
            ClassCheck classIsStaticPublicOrInternal = Class(
                Priority.Low,
                Method(
                    Priority.Mid,
                    hasStaticPublicInternalMethod
                )
            );

            return TestICheck(classIsStaticPublicOrInternal, graph);
        }

        [Test]
        public Task CallsPrivateProtectedConstructorTest()
        {
            // Create a graph with a correct private constructor call
            SyntaxGraph graph = EntityNodeUtils.CreateCorrectSingleton();
            SingletonRecognizer sr = new();

            sr.OnlyPrivateProtectedConstructor(out ConstructorCheck constructor);

            // Method to check
            ICheck callsPrivateProtectedConstructor =
                sr.CallsPrivateProtectedConstructor(constructor);

            // Put in a class, otherwise cannot be tested
            ClassCheck classCallsPrivateProtectedConstructor = Class(
                Priority.Low,
                constructor,
                Method(
                    Priority.Low,
                    callsPrivateProtectedConstructor
                )
            );

            return TestICheck(classCallsPrivateProtectedConstructor, graph);
        }

        [Test]
        public Task DoesNotCallPrivateProtectedConstructorTest()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first and the last two
            // adheres to all requirements and the second and third are missing one specific modifier
            SyntaxGraph graph = EntityNodeUtils.CreateSingletonNoConstructorCall();
            SingletonRecognizer sr = new();

            sr.OnlyPrivateProtectedConstructor(out ConstructorCheck constructor);

            // Method to check
            ICheck callsPrivateProtectedConstructor =
                sr.CallsPrivateProtectedConstructor(constructor);

            // Put in a class, otherwise cannot be tested
            ClassCheck classCallsPrivateProtectedConstructor = Class(
                Priority.Low,
                constructor,
                Method(
                    Priority.Low,
                    callsPrivateProtectedConstructor
                )
            );

            return TestICheck(classCallsPrivateProtectedConstructor, graph);
        }


        [Test]
        public Task DoesReturnPrivateFieldTest()
        {
            // Create a graph of a correct singleton which returns its private field
            SyntaxGraph graph = EntityNodeUtils.CreateCorrectSingleton();
            SingletonRecognizer sr = new();

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
            return TestICheck(classReturnsPrivateField, graph);
        }

        [Test]
        public Task DoesNotReturnPrivateFieldTest()
        {
            // Create a graph of an incorrect singleton which does not return its private field
            SyntaxGraph graph = EntityNodeUtils.CreateSingletonNoFieldUsage();
            SingletonRecognizer sr = new();
            
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

            return TestICheck(classReturnsPrivateField, graph);
        }

        [Test]
        public Task StaticMethodActsAsConstructorTestCorrect()
        {
            // Create a graph of 5 classes with 5 different singleton implementations where the first adheres to all requirements
            // and the last four are all missing one specific requirement
            SyntaxGraph graph = EntityNodeUtils.CreateCorrectSingleton();
            SingletonRecognizer sr = new();

            sr.OnlyPrivateProtectedConstructor(out ConstructorCheck privateProtectedConstructorCheck);

            FieldCheck fieldCheck = sr.StaticPrivateFieldOfTypeClass();

            // Step 3 of Singleton StepByStep
            MethodCheck hasMethodActsAsConstructor =
                sr.CheckMethodActsAsConstructorBehaviour(
                    privateProtectedConstructorCheck,
                    fieldCheck);

            return TestICheck(Class(
                Priority.Low,
                privateProtectedConstructorCheck,
                fieldCheck,
                hasMethodActsAsConstructor
            ), graph);
        }

        [Test]
        public Task StaticMethodActsAsConstructorTestWrongModifiers()
        {
            // Create a graph of a singleton class with wrong method modifiers
            SyntaxGraph graph = EntityNodeUtils.CreateSingletonWrongMethodModifiers();
            SingletonRecognizer sr = new();

            sr.OnlyPrivateProtectedConstructor(out ConstructorCheck privateProtectedConstructorCheck);

            FieldCheck fieldCheck = sr.StaticPrivateFieldOfTypeClass();

            // Step 3 of Singleton StepByStep
            MethodCheck hasMethodActsAsConstructor =
                sr.CheckMethodActsAsConstructorBehaviour(
                    privateProtectedConstructorCheck,
                    fieldCheck);

            return TestICheck(Class(
                Priority.Low, 
                privateProtectedConstructorCheck,
                fieldCheck,
                hasMethodActsAsConstructor
            ), graph);
        }

        [Test]
        public Task StaticMethodActsAsConstructorTestNoConstructorCall()
        {
            // Create a graph of a singleton class with no call to the constructor
            SyntaxGraph graph = EntityNodeUtils.CreateSingletonNoConstructorCall();
            SingletonRecognizer sr = new();

            sr.OnlyPrivateProtectedConstructor(out ConstructorCheck privateProtectedConstructorCheck);

            FieldCheck fieldCheck = sr.StaticPrivateFieldOfTypeClass();

            // Step 3 of Singleton StepByStep
            MethodCheck hasMethodActsAsConstructor =
                sr.CheckMethodActsAsConstructorBehaviour(
                    privateProtectedConstructorCheck,
                    fieldCheck);

            return TestICheck(Class(
                Priority.Low,
                privateProtectedConstructorCheck,
                fieldCheck,
                hasMethodActsAsConstructor
            ), graph);
        }

        [Test]
        public Task StaticMethodActsAsConstructorTestNoFieldUsage()
        {
            // Create a graph of a singleton class with no usage of the private field
            SyntaxGraph graph = EntityNodeUtils.CreateSingletonNoFieldUsage();
            SingletonRecognizer sr = new();

            sr.OnlyPrivateProtectedConstructor(out ConstructorCheck privateProtectedConstructorCheck);

            FieldCheck fieldCheck = sr.StaticPrivateFieldOfTypeClass();

            // Step 3 of Singleton StepByStep
            MethodCheck hasMethodActsAsConstructor =
                sr.CheckMethodActsAsConstructorBehaviour(
                    privateProtectedConstructorCheck,
                    fieldCheck);

            return TestICheck(Class(
                Priority.Low,
                privateProtectedConstructorCheck,
                fieldCheck,
                hasMethodActsAsConstructor
            ), graph);
        }

        [Test]
        public Task ClientDoesCallMethodActsAsConstructorTest()
        {
            // Create a graph of a singleton class where the client uses the singleton
            SyntaxGraph graph = EntityNodeUtils.CreateCorrectClientSingleton();
            SingletonRecognizer sr = new();

            //Method which the client should call
            MethodCheck publicInternalMethod = Method(
                Priority.Mid,
                sr.IsStaticPublicOrInternal()
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

            return TestICheck(checkBothClasses, graph);
        }

        [Test]
        public Task ClientDoesNotCallMethodActsAsConstructorTest()
        {
            // Create a graph of a singleton class where the client does not use the singleton
            SyntaxGraph graph = EntityNodeUtils.CreateWrongClientSingleton();
            SingletonRecognizer sr = new();

            //Method which the client should call
            MethodCheck publicInternalMethod = Method(
                Priority.Mid,
                sr.IsStaticPublicOrInternal()
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

            return TestICheck(checkBothClasses, graph);
        }


        Task TestICheck(ICheck check, SyntaxGraph graph)
        {
            RecognizerContext4Tests ctx = RecognizerContext4Tests.Create(graph);
            Dictionary<string, IEntity> entireTree = graph.GetAll();
            List<ICheckResult> results = new();

            foreach (KeyValuePair<string, IEntity> current in entireTree)
            {
                ICheckResult res = check.Check(
                    ctx,
                    current.Value);
                results.Add(res);
            }

            return Verifier.Verify(results);
        }

    }
}
