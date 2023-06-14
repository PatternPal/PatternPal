# Bridge
## Introduction
The bridge is a structural design pattern that lets you split a large class or a set of closely related classes into two separate hierarchies - abstraction and implementation - which can be developed independently of each other. The explanation on this page is based on the definition of previous development teams and the website _refactoring guru_[^1].

### Context of use
The bridge design pattern is used to decouple an abstraction from its implementation, allowing them to vary independently. It is useful when you want to separate the interface or abstraction of a class from its concrete implementation. This pattern promotes loose coupling between the two, making it easier to modify or extend them individually without affecting each other.

### Example of use
An example could be  a drawing application where we have different shapes to draw (e.g., circles, squares). The bridge pattern can be applied by separating the drawing functionality (implementation) from the shape objects (abstraction). The shape objects can have a reference to a drawing implementation object, which can be swapped without affecting the shape objects. Another example is the shape objects and their possible colors. ![Example picutre Bridge](https://refactoring.guru/images/patterns/diagrams/bridge/solution-en.png)

## Explanation of Architecture
![UML Bridge](https://refactoring.guru/images/patterns/diagrams/bridge/structure-en.png)

The **Abstraction** provides a high-level control logic. It relies on the implementation object to do the actual low-level work.

The **Implementation** declares the interface that's common for all concrete implementations. An abstraction can only communicate with an implementation object via methods that are declared here. 

The abstraction may list the same methods as the implementation but usually the abstraction decalares some complex behaviors that rely on a wide variety of primitive operations declared by the implementation.

The **Concerete Implementations** contain case-specific code.

The **Refined Abstractions** provide variants of control logic. Like their parent, they work with different implementations via the general implementation interface. 

Usually, the **Client** is only interested in working with the abstraction. However, it's the client's job to link the abstraction object with one of the implemetation objects. 

## Requirements
The priority of a requirement is noted with the (low)-(mid)-(high)-(knockout) criteria.

**Client class**
1. (mid) uses a method in the `Abstraction` class
2. (low) creates a `Concrete Implementation` instance
3. (low) uses the field or property in `Abstraction`

**Abstraction class**
1. (knockout) has a private / protected field or property with the `Implementation` type
2. (knockout) has a method 
3. (high) there is a method that calls a method in `Implementation`

**Implementation**
1. (knockout) is an interface or abstract class
2. (high) has at least one (abstract) method

**Concrete Implementations**
1. (high) is an implementation of the `Implementation` interface or inherits from the `Implementation` abstract class
2. (mid) if `Implementation` is an abstract class it should override it's abstract methods

**Refined Abstraction** optional
1. (low) inherits from the `Abstraction` class
2. (low) has an method

## References
[^1]: Refactoring Guru, Creational Patterns - Bridge. https://refactoring.guru/design-patterns/bridge

