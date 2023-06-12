#region

using PatternPal.Core.StepByStep;
using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a factory-method pattern needs to have implemented.
/// It returns the requirements in a tree structure stated per class.
/// </summary>
/// <remarks>
/// Requirements for the Product class:<br/>
///     a) is an interface<br/>
///     b) gets inherited by at least one class<br/>
///     c) gets inherited by at least two classes<br/>
/// Requirements for the Concrete Product class:<br/>
///     a) inherits Product<br/>
///     b) gets created in a Concrete Creator<br/>
/// Requirements for the Creator class:<br/>
///     a) is an abstract class<br/>
///     b) gets inherited by at least one class<br/>
///     c) gets inherited by at least two classes<br/>
///     d) contains a factory-method with the following properties<br/>
///         1) method is abstract<br/>
///         2) method is public<br/>
///         3) returns an object of type Product<br/>
/// Requirements for the Concrete Creator class:<br/>
///     a) inherits Creator<br/>
///     b) has exactly one method that creates and returns a Concrete product<br/>
/// </remarks>
internal class FactoryMethodRecognizer : IRecognizer
{
    /// <inheritdoc />
    public string Name => "Factory-Method";

    /// <inheritdoc />
    public Recognizer RecognizerType => Recognizer.FactoryMethod;

    /// <summary>
    /// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a singleton pattern needs to have implemented.
    /// It returns the requirements in a tree structure stated per class.
    /// </summary>
    public IEnumerable< ICheck > Create()
    {
        //Product c
        //Creator c

        //Product a
        InterfaceCheck productIsInterface = Interface(
            Priority.Knockout,
            Any(Priority.Knockout)
        );
        yield return productIsInterface;

        //Concrete product a & Product b
        RelationCheck concreteInheritsProduct = Inherits(
            Priority.Knockout,
            productIsInterface);

        //Creator a
        ModifierCheck isAbstract = Modifiers(
            Priority.Knockout,
            Modifier.Abstract
        );

        //Creator d
        MethodCheck factoryMethod = Method(
            Priority.High,
            Modifiers(
                Priority.Knockout,
                Modifier.Abstract
            ),
            Modifiers(
                Priority.High,
                Modifier.Public
            ),
            Type(
                Priority.Knockout,
                productIsInterface
            )
        );
        
        ClassCheck creator = Class(
            Priority.Low,
            isAbstract,
            //b
            //c
            factoryMethod
        );
        yield return creator;

        //Concrete Creator a & Creator b
        RelationCheck concreteInheritsCreator = Inherits(
            Priority.Knockout,
            creator
        );

        //Concrete Creator b --Todo does not check if it is exactly one
        MethodCheck methodReturnsConcreteProduct = Method(
            Priority.Low,
            Type(
                Priority.Low,
                concreteInheritsProduct
            ),
            Creates(
                Priority.Low,
                concreteInheritsProduct
            )
        );

        ClassCheck concreteCreator = Class(
            Priority.Low,
            concreteInheritsCreator,
            methodReturnsConcreteProduct
        );

        //Concrete product b
        RelationCheck getsCreatedInConcreteProduct = CreatedBy(
            Priority.High,
            concreteCreator
        );

    }

    public List<IInstruction> GenerateStepsList()
    {
        throw new NotImplementedException();
    }
}
