# Adapter
## Introduction
An adapter pattern is a design pattern which is very useful when one wants to use code which is not in the same format as the code you are currently writing. The explanation on this page is based on the definition of previous development teams, the website _refactoring guru_[^1] and the book _Design Patterns Explained_[^2].

### Context of use
The adapter pattern is very useful when one wants to use code which is not in the same format as the code you are currently writing. The adapter will create an object of the used code and convert the data so that your new program can use the old object types. The old object and the new code are not even aware of the adapter. This pattern makes sure you can reuse existing subclasses without having to rewrite the old classes.

### Example of use
In programming this could be used for example when you have a program with different types of shapes, all with the coordinates of the center, and other (possibly third party) code with a particular shape with the coordinates at the top left corner. You could write an adapter which adapts the coordinates from the top left corner to the center so that the current program does not have to rewrite the old program with the corner coordinated shapes but can just use the shapes as if they were center coordinated.

## Explanation of Architecture
![UML Adapter](https://refactoring.guru/images/patterns/diagrams/adapter/structure-object-adapter-2x.png?id=03e8052e168c962d6bc369bbb13b0945)

The **Service** class is the class in which the adapted code is located. This code should not be altered and is already written (possibly by a third party) and is not directly usable by the `Client`.
The **Client Interface** is an interface with a different structure than the `Service`. This interface contains some methods or fields that the Client wants to use and the `Adapter` will overwrite.
The **Adapter** is a class which inherits from the `Client Interface`. This class will store a `Service` object and get calls from the `Client` via the `Client Interface`. It translates these calls to such a format the `Service` can understand.
The **Client** is the class from which the `Adapter` object gets created.

## Requirements
The priority of a requirement is noted with the (low)-(mid)-(high)-(knockout) criteria.

**Service class**
1. (knockout) does not inherit from the Client Interface
2. (high) is used by the Adapter class

**Client class**
1. (mid) has created an object of the type Adapter
2. (low) has used a method of the Service via the Adapter

**Client interface**
1. (knockout) is an interface/abstract class
2. (knockout) is inherited/implemented by an Adapter
3. (high) contains a method
    1. (high) if it is an abstract class the method should be abstract

**Adapter**
1. (knockout) inherits/implements the Client Interface
2. (knockout) creates an Service object or gets one via the constructor
3. (knockout) contains a private field in which the Service is stored
4. (high) does not return an instance of the Service
5. (high) a method uses the Service class


## References
[^1]: Refactoring Guru, Creational Patterns - Adapter. https://refactoring.guru/design-patterns/adapter
[^2]: A.Shalloway, J.R.Trott, Design Patterns Explained - A new perspective on Object Oriented Design (p.102 - 104). (2004)
