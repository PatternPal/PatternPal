# Factory-Method
## Introduction
A Factory-Method pattern is a design pattern which is very useful when one wants to reduice code duiplication when creating different objects with the same characteristics. The explanation on this page is based on the definition of previous development teams, the website _refactoring guru_[^1] and the book _Design Patterns Explained_[^2].

### Context of use
The Factory-Method is great for when a class needs to instantiate a derivation of another class, but doesn't
know which one. Factory Method allows a derived class to make this decision. Via the Factory-Method you can easily reduce code duplication because the overlapping things can be written in an abstract class and every different derivation can use these overlapping methods. The different things can be implemented via an overwrite of the default class. With this pattern you will only have to write every piece of code one time which makes the code more maintainable.

### Example of use
An example of use for this pattern is creating a button on a different IOS. You create a ‘Creator’ class in which you can write all the overlapping information. Then every ‘ConcreteCreator’ class will inherit from the Creator class and overwrite the things which are different for its own. For example, the text on the button can be written in the superclass Creator, but the behavior of the button (for example ‘go to settings’) will be written in the subclass because this is different for each IOS

## Explanation of Architecture
![UML Factory-Method](https://refactoring.guru/images/patterns/diagrams/factory-method/example-2x.png?id=a2470830778e318263155000dbdc5870)

The **Service** class is the class in which the adapted code is located. This code should not be altered and is already written (possibly by a third party) and is not directly usable by the `Client`.
The **Product** interface is the interface in which the properties of the `Concrete Products` gets declared.
The **Concrete** Product classes are the specific products that the corresponding `Concrete Creators` will create.
The **Creator** class declares the factory which will contain the factory method which will create a `Product`.
The **ConcreteCreator** class is a specific implementation of the Creator class which will specify how to create its corresponding `Concrete Product`.

## Requirements
The priority of a requirement is noted with the (low)-(mid)-(high)-(knockout) criteria.

**Product class**
1. (knockout) is an interface
2. (knockout) gets inherited by at least one class
3. (mid) gets inherited by at least two classes

**Concrete Product class**
1. (knockout) inherits Product
2. (high) gets created in a Concrete Creator

**Creator class**
1. (knockout) is an abstract class
2. (knockout) gets inherited by at least one class 
3. (mid) gets inherited by at least two classes
4. (-) contains a factory-method with the following properties
    1. (knockout) method is abstract
    2. (high) method is public
    3. (knockout) returns an object of type Product

**Concrete Creator class**
1. (knockout) inherits Creator
2. (low) has exactly one method that creates and returns a Concrete product

## Step By Step Steps
| Steps | Requirements                                                                                                                                                                                                        | Explanations                                                                                                                                                                                                                                                                                                                                                                                         |
|-------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1     | Create an interface. We refer to this interface as the Product.                                                                                                                                                     | It needs to be an interface as all the properties of all concrete products get declared in this interface.                                                                                                                                                                                                                                                                                           |
| 2     | Create two new classes which inherit the Product. We refer to these classes as Concrete Product.                                                                                                                    | It need to be two classes because using the factory method pattern for just one concrete product is not useful. It also needs to inherit product as the properties of a concrete product are declared there. (PatternPal only checks for one class)                                                                                                                                                  |
| 3     | Create an abstract class which will contain an public abstract method which will return something of type Product. This class will be referred to as Creator.                                                       | The creator is an abstract class and not an interface because this class also has functions outside of creating a product. Only the method responsible for creating a product, the factoryMethod(), is abstract as this is the only function of the concrete creator classes.                                                                                                                        |
| 4     | Create two classes which will inherit from the Creator class. Each class will have exactly one method which will create and return a different concrete product. This class will be referred to as Concrete Creator | It need to be two classes because every concrete product should have its own concrete creator and there are at least two concrete products. Each concrete creator is a factory for one concrete product. and creating that concrete product is the only function of this class. It needs to inherit creator as the factoryMethod() method is declared there.  (PatternPal only checks for one class) |

## References
[^1]: Refactoring Guru, Creational Patterns - Factory-Method. https://refactoring.guru/design-patterns/factory-method
[^2]: A.Shalloway, J.R.Trott, Design Patterns Explained - A new perspective on Object Oriented Design (p.288). (2004)
