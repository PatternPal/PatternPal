#region

using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided files or project implements the adapter pattern
/// </summary>
/// <remarks>
/// Requirements to fulfill the pattern:<br/>
/// </remarks>
internal abstract class AdapterRecognizerParent : IRecognizer
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
    ///     c) has not used a method of the Service without the adapter<br/>
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
    ///     d) uses the Service class<br/>
    ///     e) does not return an instance of the Service<br/>
    ///     f) every method uses the Service class<br/>
    /// </remarks>

    public IEnumerable< ICheck > Create()
    {
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
        RelationCheck createsServiceObject = CreatesObject(clientInterfaceClassType);

        //Check Adapter c
        FieldCheck containsServiceField = ContainsServiceField(clientInterfaceClassType);

        //Check Adapter d
        ICheck noServiceReturn = NoServiceReturn(service);

        //Check Adapter e, Service b
        MethodCheck usesServiceClass = Method(
            Priority.High,
            Uses(
                Priority.High,
                service
            )
        );

        //Check Adapter f
        ICheck allUsesServiceClass = All(
            Priority.Mid,
            usesServiceClass
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
            allUsesServiceClass,
            adapterMethodsUsingService
        );

        //Check Client a
        RelationCheck createsAdapter = Creates(
            Priority.Mid,
            adapter
        );

        //Check Client b
        RelationCheck usedService = Uses(
            Priority.Low,
            adapterMethodsUsingService
        );

        //Check Client c
        ICheck notUsedService = Method(Priority.Low);//TODO: Implemnt check

        yield return service;

        yield return adapter;

        yield return Class(
            Priority.Low,
            clientInterfaceMethod,
            clientInterfaceClassType
        );

        yield return Class(
            Priority.Low,
            createsAdapter,
            usedService,
            notUsedService
        );
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

    public abstract FieldCheck ContainsServiceField(ICheck service);

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

    ICheck UsesServiceMethodViaAdapter(MethodCheck allServiceMethods)
    {
        
    }
}
