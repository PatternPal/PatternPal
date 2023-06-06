#region
using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the adapter pattern
/// </summary>
/// <remarks>
/// Requirements to fulfill the pattern:<br/>
/// </remarks>
internal abstract class AdapterRecognizerParent
{
    /// <summary>
    /// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a adapter pattern needs to have implemented.
    /// It returns the requirements in a tree structure stated per class.
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
    ///     b) creates an Service object<br/>
    ///     c) contains a private field in which the Service is stored<br/>
    ///     d) does not return an instance of the Service<br/>
    ///     e) a method uses the Service class<br/>
    ///     f) every method uses the Service class<br/>
    /// </remarks>

    public ICheck[] Checks()
    {
        ICheck[] result = new ICheck[4];

        //Check Client interface c, ci
        MethodCheck clientInterfaceMethod = ContainsMaybeAbstractVirtualMethod();

        //Check Client interface a
        ICheck clientInterfaceClassType = IsInterfaceAbstractClassWithMethod(clientInterfaceMethod);

        //Helps check Client b
        MethodCheck allServiceMethods = Method(Priority.Low);

        //Check Concrete Service a
        ClassCheck service = Class(
            Priority.Low,
            Not(
                Priority.Knockout,
                DoesInheritFrom(clientInterfaceClassType)
            ),
            allServiceMethods
        );

        //Check Adapter a, Client interface b
        RelationCheck inheritsClientInterface = DoesInheritFrom(clientInterfaceClassType); //TODO: vraag linde checkbuilder

        //Check Adapter b
        RelationCheck createsServiceObject = CreatesObject(service);

        //Check Adapter c
        FieldCheck containsServiceField = ContainsServiceField(service);

        //Check Adapter d
        ICheck noServiceReturn = NoServiceReturn(service);

        //Check Adapter e, Service b
        ICheck usesServiceClass = Method(
            Priority.High,
            UsesServiceClass(service, containsServiceField)
        );

        //Check Adapter f
        ICheck allUsesServiceClass = AllMethodsUseServiceClass(service, containsServiceField);

        //Helps Client b
        MethodCheck adapterMethodsUsingService = Method(
            Priority.Low,
            Uses(
                Priority.Low,
                allServiceMethods
            )
        );

        ClassCheck adapter = Class(
            Priority.Low,
            inheritsClientInterface,
            createsServiceObject,
            containsServiceField,
            noServiceReturn,
            usesServiceClass,
            allUsesServiceClass,
            adapterMethodsUsingService
        );

        //Check Client a
        RelationCheck createsAdapter = Creates(
            Priority.Mid,
            adapter
        );

        //Check Client b
        MethodCheck clientUsesServiceViaAdapter = Method(
            Priority.Low,
            Uses(
                Priority.Low,
                adapterMethodsUsingService
            )
        );

        result[0] = clientInterfaceClassType;

        result[1] = service;

        result[2] = adapter;

        result[3] = Class(
            Priority.Low,
            createsAdapter,
            clientUsesServiceViaAdapter
        );

        return result;
    }



    public abstract ICheck IsInterfaceAbstractClassWithMethod(MethodCheck method);

    public abstract MethodCheck ContainsMaybeAbstractVirtualMethod();

    public abstract RelationCheck DoesInheritFrom(ICheck parent);

    private RelationCheck CreatesObject(ICheck obj)
    {
        return Creates(
            Priority.Knockout,
            obj
        );
    }

    public FieldCheck ContainsServiceField(ClassCheck service)
    {
        {
            return Field(
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
    }

    ICheck NoServiceReturn(ClassCheck service)
    {
        return Not(
            Priority.High,
            Method(
                Priority.High,
                Type(
                    Priority.High,
                    service
                )
            )
        );
    }

    ICheck UsesServiceClass(ClassCheck service, FieldCheck serviceField)
    {
        return Any(
            Priority.High,
            Uses(
                Priority.High,
                service
            ),
            Uses(
                Priority.High,
                serviceField
            )
        );
    }

    ICheck AllMethodsUseServiceClass(ClassCheck service, FieldCheck serviceField)
    {
        return Not(
            Priority.Mid,
            Method(
                Priority.Mid,
                Not(
                    Priority.Mid,
                    UsesServiceClass(service, serviceField)
                )
            )
        );
    }
}
