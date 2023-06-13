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
///     c) gets inherited by at least two classes<br/>                          --todo: check this
/// Requirements for the Concrete Product class:<br/>
///     a) inherits Product<br/>
///     b) gets created in a Concrete Creator<br/>
/// Requirements for the Creator class:<br/>
///     a) is an abstract class<br/>
///     b) gets inherited by at least one class<br/>
///     c) gets inherited by at least two classes<br/>                          --todo: check this
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
    public IEnumerable<ICheck> Create()
    {
        InterfaceCheck product = Product();

        ClassCheck creator = Creator(product);

        ClassCheck concreteProduct = ConcreteProduct(product);

        yield return product;
        yield return concreteProduct;
        yield return creator;
        yield return ConcreteCreator(creator, concreteProduct);
    }

    InterfaceCheck Product()
    {
        //Product a
        return Interface(
            Priority.Knockout,
            "is an interface"
        );
    }

    ClassCheck Creator(InterfaceCheck product)
    {
        //Creator a
        ModifierCheck isAbstract = Modifiers(
            Priority.Knockout,
            "is an abstract class",
            Modifier.Abstract
        );

        //Creator d
        MethodCheck factoryMethod = Method(
            Priority.High,
            "contains a factory-method with the following properties: 1) method is abstract 2) method is public 3) returns an object of type Product",
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
                product
            )
        );

        return Class(
            Priority.Low,
            isAbstract,
            factoryMethod
        );
    }

    ClassCheck ConcreteProduct(InterfaceCheck product)
    {
        //Concrete product a & Product b
        RelationCheck concreteInheritsProduct = Implements(
            Priority.Knockout,
            "gets inherited by at least one class + Concrete Product inherits Product",
            product);

        return Class(
            Priority.Low,
            concreteInheritsProduct
        );
    }

    ClassCheck ConcreteCreator(ClassCheck creator, ClassCheck concreteProduct)
    {
        //Concrete Creator a & Creator b
        RelationCheck concreteInheritsCreator = Inherits(
            Priority.Knockout,
            "inherits Creator + Creator gets inherited by at least one class",
            creator
        );

        //Concrete Creator b --Todo does not check if it is exactly one
        MethodCheck methodReturnsConcreteProduct = Method(
            Priority.Low,
            "has exactly one method that creates and returns a Concrete product",
            Type(
                Priority.Low,
                concreteProduct
            ),
            Creates(
                Priority.Low,
                concreteProduct
            )
        );

        //Concrete product b
        RelationCheck createsConcreteProduct = Creates(
            Priority.High,
            "gets created in a Concrete Creator",
            concreteProduct
        );

        return Class(
            Priority.Low,
            concreteInheritsCreator,
            methodReturnsConcreteProduct,
            createsConcreteProduct
        );
    }

    public List<IInstruction> GenerateStepsList()
    {
        throw new NotImplementedException();
    }
}
