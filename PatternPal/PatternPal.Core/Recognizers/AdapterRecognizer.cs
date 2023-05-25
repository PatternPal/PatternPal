#region

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
internal class AdapterRecognizer : IRecognizer
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
        
    }

    ClassCheck IsInterfaceAbstractClass()
    {
        return Class(
            Priority.Knockout,
            Type(
                Priority.Knockout,
                )
            )
    }


}
