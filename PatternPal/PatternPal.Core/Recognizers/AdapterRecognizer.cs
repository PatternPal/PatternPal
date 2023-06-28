#region

using System.Runtime.ExceptionServices;
using PatternPal.Core.StepByStep;
using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;
using Method = Google.Protobuf.WellKnownTypes.Method;
using PatternPal.Core.StepByStep.Resources.Instructions;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implement the adapter pattern
/// </summary>
/// <remarks>
/// Requirements for the Service class:<br/>
///     a) does not inherit from the Client Interface.<br/>
///     b) is used by the Adapter class<br/>
/// <br/>
/// Requirements for the Client class:<br/>
///     a) has created an object of the type Adapter<br/>
///     b) has used a method of the Service via the Adapter<br/>
/// <br/>
/// Requirements for the Client interface:<br/>
///     a) is an interface/abstract class<br/>
///     b) is inherited/implemented by an Adapter<br/>
///     c) contains a method<br/>
///         i) if it is an abstract class the method should be abstract or virtual<br/>
/// <br/>
/// Requirements for the Adapter class:<br/>
///     a) inherits/implements the Client Interface<br/>
///     b) creates an Service object or gets one via the constructor<br/>
///     c) contains a private field in which the Service is stored<br/>
///     d) does not return an instance of the Service<br/> //TODO this is currently checked only for possible methods, but a property could also return the service
///     e) a method uses the Service class<br/>
/// </remarks>
internal class AdapterRecognizer : IRecognizer
{
    /// <inheritdoc />
    public string Name => "Adapter";

    /// <inheritdoc />
    public Recognizer RecognizerType => Recognizer.Adapter;

        /// <inheritdoc />
        IEnumerable< ICheck > IRecognizer.Create()
        {
            //The Adapter utilizing an interface
            AdapterRecognizerParent isInterface = new AdapterRecognizerInterface();
            //The Adapter utilizing an interface
            AdapterRecognizerParent isAbstractClass = new AdapterRecognizerAbstractClass();

            yield return 
                Any(
                    Priority.Knockout,
                    All(
                        Priority.Knockout,
                        isAbstractClass.Checks()
                    ),
                    All(
                        Priority.Knockout,
                        isInterface.Checks()
                    )
                );
        }
}
/// <summary>
/// An abstract class which contains all checks for the adapter<see cref="IRecognizer"/>.
/// Because the adapter-pattern can be applied using an interface or an abstract class, some requirements are interface or abstract class specific and defined in the subclasses.
/// </summary>
abstract file class AdapterRecognizerParent
{
    public ICheck[] Checks()
    { 
        //Check Client interface c, ci
        MethodCheck clientInterfaceMethod = ContainsOverridableMethod();

        //Check Client interface a
        ICheck clientInterfaceClassType = IsInterfaceOrAbstractClassWithMethod(clientInterfaceMethod);

        //Helps check Client b
        MethodCheck serviceMethod = ServiceMethod();

        //Check Concrete Service a
        ClassCheck service = Service(serviceMethod, clientInterfaceClassType);

        //Check Adapter c
        FieldCheck containsServiceField = ContainsServiceField(service);

        MethodCheck adapterMethod = AdapterMethod(containsServiceField, serviceMethod);

        ICheck createsObjectOrGetViaConstructor = CreateServiceOrGetViaConstructor(service, containsServiceField);

        ClassCheck adapter = Adapter(clientInterfaceClassType, createsObjectOrGetViaConstructor, service, containsServiceField, adapterMethod);

        ICheck client = Client(adapter, adapterMethod);

        return new[] { clientInterfaceClassType, service, adapter, client };
    }

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if an <see cref="INode"/> is either an <see langword="interface"/> or an <see langword="abstract class"/>.
    /// </summary>
    public abstract ICheck IsInterfaceOrAbstractClassWithMethod(MethodCheck method);

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if there exists a method in the Client Interface."/>.
    /// If the Client Interface is an <see langword="abstract class"/> then it also checks if the method is <see langword="abstract"/>.
    /// </summary>
    public abstract MethodCheck ContainsOverridableMethod();

    /// <summary>
    /// An <see cref="ICheck"/> searching for the <see cref="IMethod"/> of Service. 
    /// </summary>
    public MethodCheck ServiceMethod()
    {
        return Method(Priority.Low);
    }

    /// <summary>
    /// An <see cref="ICheck"/> searching for the Service <see cref="IClass"/>, encompassing all requirements of the Service class.
    /// It should have an <see cref="IMethod"/> and may not inherit from  / implement ClientInterface
    /// </summary>
    public ClassCheck Service(MethodCheck serviceMethod, ICheck clientInterfaceClassType)
    {
        return Class(
            Priority.Low,
            Not(
                Priority.Knockout,
                DoesInheritFrom(clientInterfaceClassType)
            ),
            serviceMethod
        );
    }

    /// <summary>
    /// An <see cref="ICheck"/> searching for the <see cref="IMethod"/> of Adapter. Checks whether the method uses the method of Service via the field.
    /// </summary>
    protected MethodCheck AdapterMethod(FieldCheck serviceField, MethodCheck serviceMethod)
    {
        return
            Method(
                Priority.High,
                Uses(
                    Priority.High,
                    serviceField
                ),
                Uses(
                    Priority.Low,
                    serviceMethod
                )
            );
    }

    /// <summary>
    /// An <see cref="ICheck"/> searching for the Adapter <see cref="IClass"/>, encompassing all requirements of the Adapter class.
    /// </summary>
    protected ClassCheck Adapter(ICheck clientInterfaceClassType, ICheck createsObjectOrGetViaConstructor, ClassCheck service, FieldCheck containsServiceField, MethodCheck adapterMethod)
    {
        return Class(
            Priority.Low,
            DoesInheritFrom(clientInterfaceClassType),
            containsServiceField,
            createsObjectOrGetViaConstructor,
            Not(
                Priority.High,
                Method(
                    Priority.High,
                    Type(
                        Priority.High,
                        service
                    )
                )
            ),
            adapterMethod
        );
    }

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck inherits from the <paramref name="parent"/> node.
    /// </summary>
    public abstract RelationCheck DoesInheritFrom(ICheck parent);

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck creates the <paramref name="service"/> node 
    /// </summary>
    protected RelationCheck CreateService(ClassCheck service)
    {
        return Creates(
            Priority.Knockout,
            service
        );
    }

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck is a parameter of all constructors, and the parameter gets passed to the field.
    /// </summary>
    protected ICheck GetServiceFromConstructor(ClassCheck service, FieldCheck serviceField)
    {
        return All( //TODO check for field usage 
            Priority.Knockout,
            Constructor(
                Priority.Knockout,
                Parameters(
                    Priority.Knockout,
                    Type(
                        Priority.Knockout,
                        service
                    )
                ),
                Uses(
                    Priority.Knockout,
                    serviceField
                )
            ),
            Not(
                Priority.Knockout,
                Constructor(
                    Priority.Knockout,
                    Not(
                        Priority.Knockout,
                        Parameters(
                            Priority.Knockout,
                            Type(
                                Priority.Knockout,
                                service
                            )
                        )
                    )
                )
            )
        );
    }

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck creates the <paramref name="service"/> node or if it is a parameter of all constructors,
    /// and the parameter gets passed to the field
    /// </summary>
    protected ICheck CreateServiceOrGetViaConstructor(ClassCheck service, FieldCheck serviceField)
    {
        return Any(
            Priority.Knockout,
            CreateService(service),
            GetServiceFromConstructor(service, serviceField)
        );
    }

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck contains a private field with the same type as <paramref name="service"/>.
    /// </summary>
    public FieldCheck ContainsServiceField(ClassCheck service)
    {
        return 
            Field(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Private
                ),
                Type(
                    Priority.Knockout,
                    service
                )
            );
    }

    /// <summary>
    /// An <see cref="ICheck"/> searching for the Client <see cref="IClass"/>, encompassing all requirements of the Client class.
    /// It should create and use the Adapter.
    /// </summary>
    protected ClassCheck Client(ClassCheck adapter, MethodCheck adapterMethod)
    {
        return Class(
            Priority.Low,
            Creates(
                Priority.Mid,
                adapter
            ),
            Method(
                Priority.Low,
                Uses(
                    Priority.Low,
                    adapterMethod
                )
            )
        );
    }
}


/// <summary>
/// A subclass of <see cref="AdapterRecognizerParent"/> that is used to specify checks for the adapter implementation with a Client Interface with type <see langword="abstract class"/>.
/// </summary>
file class AdapterRecognizerAbstractClass : AdapterRecognizerParent
{
    /// <inheritdoc />
    public override ClassCheck IsInterfaceOrAbstractClassWithMethod(MethodCheck method)
    {
        return AbstractClass(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Abstract),
            method
        );
    }

    /// <inheritdoc />
    public override MethodCheck ContainsOverridableMethod()
    {
        return Method(
            Priority.High,
            Any(
                Priority.High,
                Modifiers(
                    Priority.High,
                    Modifier.Abstract
                ),
                Modifiers(
                    Priority.High,
                    Modifier.Virtual
                )
            )
        );
    }

    /// <inheritdoc />
    public override RelationCheck DoesInheritFrom(ICheck possibleParent)
    {
        return Inherits(
            Priority.Knockout,
            possibleParent
        );
    }
}


/// <summary>
/// A subclass of <see cref="AdapterRecognizerParent"/> that is used to specify checks for the adapter implementation with a Client Interface with type <see langword="interface"/>.
/// </summary>
file class AdapterRecognizerInterface : AdapterRecognizerParent, IStepByStepRecognizer 
{
    /// <inheritdoc />
    public string Name => "Adapter with interface";

    /// <inheritdoc />
    public Recognizer RecognizerType => Recognizer.Adapter;

    /// <inheritdoc />
    public override InterfaceCheck IsInterfaceOrAbstractClassWithMethod(MethodCheck method)
    {
        return Interface(
            Priority.Knockout,
            method
        );
    }

    /// <inheritdoc />
    public override MethodCheck ContainsOverridableMethod()
    {
        return Method(Priority.High);   
    }

    /// <inheritdoc />
    public override RelationCheck DoesInheritFrom(ICheck parent)
    {
        return Implements(
            Priority.Knockout,
            parent
        );
    }

    /// <inheritdoc />
    public List<IInstruction> GenerateStepsList()
    {
        List<IInstruction> generateStepsList = new();

        MethodCheck adapterInterfaceMethod = ContainsOverridableMethod();
        ICheck adapterInterface = IsInterfaceOrAbstractClassWithMethod(adapterInterfaceMethod);

        generateStepsList.Add(
            new SimpleInstruction(
                AdapterInstructions.Step1,
                AdapterInstructions.Explanation1,
                new List<ICheck> { adapterInterface }));

        MethodCheck serviceMethod = ServiceMethod();
        ClassCheck service = Service(serviceMethod, adapterInterface);

        generateStepsList.Add(
            new SimpleInstruction(
                AdapterInstructions.Step2,
                AdapterInstructions.Explanation2,
                new List<ICheck>
                {
                    adapterInterface,
                    service
                }));

        FieldCheck containsServiceField = ContainsServiceField(service);

        MethodCheck adapterMethod = AdapterMethod(containsServiceField, serviceMethod);
        RelationCheck createsService = CreateService(service);
        ClassCheck adapter = Adapter(adapterInterface, createsService, service, containsServiceField, adapterMethod);

        generateStepsList.Add(
            new SimpleInstruction(
                AdapterInstructions.Step3,
                AdapterInstructions.Explanation3,
                new List<ICheck>
                {
                    adapterInterface,
                    service,
                    adapter
                }));

        ICheck client = Client(adapter, adapterMethod);

        generateStepsList.Add(
            new SimpleInstruction(
                AdapterInstructions.Step4,
                AdapterInstructions.Explanation4,
                new List<ICheck>
                {
                    adapterInterface,
                    service,
                    adapter,
                    client
                }));

        return generateStepsList;
    }
}
