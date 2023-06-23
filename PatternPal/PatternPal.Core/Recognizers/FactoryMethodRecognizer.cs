#region

using PatternPal.Core.StepByStep;

using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A <see cref="IRecognizer"/> that is used to determine if the provided file is an implementation
/// of the factory-method pattern.
/// </summary>
/// <remarks>
/// Requirements for the Product class:<br/>
///     a) is an interface<br/>
///     b) gets inherited by at least one class<br/>
///     c) gets inherited by at least two classes<br/>                          --todo: check this, not possible in current test suite
/// Requirements for the Concrete Product class:<br/>
///     a) inherits Product<br/>
///     b) gets created in a Concrete Creator<br/>
/// Requirements for the Creator class:<br/>
///     a) is an abstract class<br/>
///     b) gets inherited by at least one class<br/>
///     c) gets inherited by at least two classes<br/>                          --todo: check this, not possible in current test suite
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
    /// <inheritdoc />
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

    /// <summary>
    /// Creates a <see cref="InterfaceCheck"/> that checks if there exists an interface.
    /// This method corresponds to Step 1 in the step-by-step mode: "Create an interface. We refer to this interface as the Product."
    /// </summary>
    /// <returns>A <see cref="InterfaceCheck"/> that checks for the existence of an interface</returns>
    InterfaceCheck Product()
    {
        //Product a
        return Interface(
            Priority.Knockout,
            "1. Product Interface"
        );
    }

    /// <summary>
    /// Creates a <see cref="ClassCheck"/> which checks if a class is implementing the <param name="product"></param>> interface.
    /// This method corresponds to Step 2 in the step-by-step mode: "Create two new classes which inherit the Product.
    /// We refer to these classes as Concrete Product."
    /// </summary>
    /// <param name="product"> this should be an interface check for the product interface.</param>
    /// <returns>A <see cref="ClassCheck"/> which should result in the Concrete Product class.</returns>
    ClassCheck ConcreteProduct(InterfaceCheck product)
    {
        //Concrete product a & Product b
        RelationCheck concreteInheritsProduct = Implements(
            Priority.Knockout,
            "2a. Inherits Product.",
            product);

        return Class(
            Priority.Low,
            "2. Concrete Product Class",
            concreteInheritsProduct
        );
    }

    /// <summary>
    /// Creates a <see cref="ClassCheck"/> that checks if a class is abstract and if it contains a "Factory Method".
    /// This method needs to be abstract and public, and should return an object of type "Product"
    /// This method corresponds to Step 3 in the step-by-step mode: "Create an abstract class which will contain an
    /// public abstract method which will return something of type Product. This class will be referred to as Creator."
    /// </summary>
    /// <param name="product"> this should be an interface check for the product interface.</param>
    /// <returns>A <see cref="ClassCheck"/> which should result in the Creator class.</returns>
    ClassCheck Creator(InterfaceCheck product)
    {
        //Creator a
        ModifierCheck isAbstract = Modifiers(
            Priority.Knockout,
            "3a. Is abstract.",
            Modifier.Abstract
        );

        //Creator d
        MethodCheck factoryMethod = Method(
            Priority.High,
            "3b. Contains a factory-method with the following properties: 1) method is abstract 2) method is public 3) returns an object of type Product.",
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
            "3. Creator Class",
            isAbstract,
            factoryMethod
        );
    }

    /// <summary>
    /// Creates a <see cref="ClassCheck"/> which checks multiple things about a class. It checks if the class inherits the <param name="creator"></param>
    /// class, it checks if it creates a concrete product and it checks if it contains a method which will create and return a concrete product.
    /// This method corresponds to Step 4 in the step-by-step mode: "Create two classes which will inherit from the Creator class. Each class will have
    /// exactly one method which will create and return a different concrete product. This class will be referred to as Concrete Creator"
    /// </summary>
    /// <param name="creator"> this should be a class check of the class Creator</param>
    /// <param name="concreteProduct"> this should be a class check of the class Concrete Product</param>
    /// <returns>A <see cref="ClassCheck"/> which should result in the Concrete Creator class.</returns>
    ClassCheck ConcreteCreator(ClassCheck creator, ClassCheck concreteProduct)
    {
        //Concrete Creator a & Creator b
        RelationCheck concreteInheritsCreator = Inherits(
            Priority.Knockout,
            "4a. Inherits Creator.",
            creator
        );

        //Concrete Creator b --Todo does not check if it is exactly one
        MethodCheck methodReturnsConcreteProduct = Method(
            Priority.Low,
            "4b. Has exactly one method that creates and returns a Concrete product.",
            Type(
                Priority.Low,
                concreteProduct
            ),
            Creates(
                Priority.Low,
                concreteProduct
            )
        );
        //Looks like duplication of the check above, but this one has a higher priority and is more often correct than the check above
        //Concrete product b
        RelationCheck createsConcreteProduct = Creates(
            Priority.High,
            "2b. Gets created in a Concrete Creator.",
            concreteProduct
        );

        return Class(
            Priority.Low,
            "4. Concrete Creator Class",
            concreteInheritsCreator,
            methodReturnsConcreteProduct,
            createsConcreteProduct
        );
    }
}
