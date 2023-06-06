#region
using PatternPal.SyntaxTree.Models;
using static PatternPal.Core.Checks.CheckBuilder;
#endregion

namespace PatternPal.Core.Recognizers.Helper_Classes;

/// <summary>
/// An abstract class which contains all checks for the adapter<see cref="IRecognizer"/>.
/// Because the adapter-pattern can be applied using an interface or an abstract class, some methods are abstract and specified in the subclass.
/// </summary>
internal abstract class AdapterRecognizerParent
{
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
        RelationCheck inheritsClientInterface = DoesInheritFrom(clientInterfaceClassType);

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


    /// <summary>
    /// An <see cref="ICheck"/> which will determine if a <see langword="class"/> is either an <see langword="interface"/> or an <see langword="abstract class"/>.
    /// </summary>
    public abstract ICheck IsInterfaceAbstractClassWithMethod(MethodCheck method);

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if there exists a method in the Client Interface."/>.
    /// If the Client Interface is an <see langword="abstract class"/> then it also checks if the method is <see langword="abstract"/>.
    /// </summary>
    public abstract MethodCheck ContainsMaybeAbstractVirtualMethod();

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck inherits from the <see cref="parent"/> node.
    /// </summary>
    public abstract RelationCheck DoesInheritFrom(ICheck parent);

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck creates the <see cref="obj"/> node.
    /// </summary>
    private RelationCheck CreatesObject(ICheck obj)
    {
        return Creates(
            Priority.Knockout,
            obj
        );
    }

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentcheck contains the <see cref="service"/> node in a field.
    /// </summary>
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

    /// <summary>
    /// An <see cref="ICheck"/> which will determine if the parentclass does not return an object of the <see cref="service"/> type.
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
    /// An <see cref="ICheck"/> which will determine if the parentcheck ueses the <see cref="service"/> class directly or via the <see cref="serviceField"/> field.
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

    /// <summary>
    /// An <see cref="ICheck"/> which determines if all methods of the parentcheck class succeed the <see cref="UsesServiceClass"/> check.
    /// </summary>
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
