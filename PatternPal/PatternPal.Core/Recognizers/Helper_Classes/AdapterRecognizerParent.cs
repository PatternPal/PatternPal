#region

using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// An abstract class which contains all checks for the adapter<see cref="IRecognizer"/>.
/// Because the adapter-pattern can be applied using an interface or an abstract class, some requirements are interface or abstract class specific and defined in the subclasses.
/// </summary>
internal abstract class AdapterRecognizerParent
{
    public ICheck[] Checks()
    {
        //Check Client interface c, ci
        MethodCheck clientInterfaceMethod = ContainsOverridableMethod();

        //Check Client interface a
        ICheck clientInterfaceClassType = IsInterfaceOrAbstractClassWithMethod(clientInterfaceMethod);

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
        RelationCheck inheritsClientInterface = DoesInheritFrom(clientInterfaceClassType);

        //Check Adapter b
        ICheck createsServiceObject = CreatesObjectOrGetViaConstructor(service);

        //Check Adapter c
        FieldCheck containsServiceField = ContainsServiceField(service);

        //Check Adapter d
        ICheck noServiceReturn = NoServiceReturn(service);

        //Check Adapter e, Service b
        ICheck usesServiceClass = Method(
            Priority.High,
            UsesServiceClass(service, containsServiceField)
        );

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
        
        ICheck client =  Class(
            Priority.Low,
            createsAdapter,
            clientUsesServiceViaAdapter
        );

        return new ICheck[] {clientInterfaceClassType, service, adapter, client};
    }


    /// <summary>
    /// An <see cref="ICheck"/> which will determine if a <see langword="node"/> is either an <see langword="interface"/> or an <see langword="abstract class"/>.
    /// </summary>
    public abstract ICheck IsInterfaceOrAbstractClassWithMethod(MethodCheck method);

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if there exists a method in the Client Interface."/>.
    /// If the Client Interface is an <see langword="abstract class"/> then it also checks if the method is <see langword="abstract"/>.
    /// </summary>
    public abstract MethodCheck ContainsOverridableMethod();

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck inherits from the <paramref name="parent"/> node.
    /// </summary>
    public abstract RelationCheck DoesInheritFrom(ICheck parent);

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck creates the <paramref name="obj"/> node or if it is given als parameter with a constructor.
    /// In the second case it also checks if there is no constructor without the  <paramref name="obj"/> type.
    /// </summary>
    private ICheck CreatesObjectOrGetViaConstructor(ClassCheck obj)
    {
        return Any(
            Priority.Knockout,
            Creates(
                Priority.Knockout,
                obj
            ),
            All(
                Priority.Knockout,
                Constructor(
                    Priority.Knockout,
                    Parameters(
                        Priority.Knockout,
                        Type(
                            Priority.Knockout,
                            obj
                        )
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
                                    obj
                                )
                            )
                        )
                    )
                )
            )   
        );
    }

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck contains a private field with the same type as <paramref name="service"/>.
    /// </summary>
    public FieldCheck ContainsServiceField(ClassCheck service)
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

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentclass does not have a method which returns an object of the <paramref name="service"/> type.
    /// </summary>
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

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck uses the <paramref name="service"/> class directly or via the <paramref name="serviceField"/> field.
    /// </summary>
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
}
