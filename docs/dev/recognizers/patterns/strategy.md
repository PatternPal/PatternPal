# Strategy
## Introduction
The strategy pattern is a behavioral design pattern hat lets you define a family of algorithms, 
put each of them into a separate class, and make their objects interchangeable. The explanation on 
this page is based on the definition of previous development teams, the website _refactoring guru_[^1] 
and the book _Design Patterns Explained_[^2].

### Context of use
The strategy pattern comes in handy when multiple methods do the same thing in different ways, this 
pattern is in this case used to reduce code duplication. The gang of four describes the strategy pattern 
in the following manner: “Define a family of algorithms, encapsulate each one, and make them interchangeable. 
Strategy lets the algorithm vary independently from clients that use it." It enables the user to use different
business rules or algorithms depending on the context in which they occur.

### Example of use
An example case that shows the need of the pattern is when someone uses a route calculator which 
calculates the shortest route from A to B. Someone then might want to use the strategy pattern for each of the 
different routing options (Bike, Car, etc). Then the code can just call a function CreateRoute independent of 
which routing algorithm is selected.

## Explanation of Architecture
![UML Strategy](https://refactoring.guru/images/patterns/diagrams/strategy/structure.png)

The **Context** is the class from where the code gets executed. It stores a reference to one of the concrete strategies. 
It only communicates with the object via the methods in the Strategy.

The **Strategy** could be an interface or an abstract class. 
This instance needs to contain a (abstract) method that will be used to execute the algorithm as implemented in the ConcreteStrategie classes.

The **ConcreteStrategy** class is designed for one specific purpose and implements/inherits the interface Strategy. 
This class implements the `execute()` function and with that defines the algorithm in its own way.

The **Client** creates a ConcreteStrategy object and passes it to the Context. 
The Context exposes a setter `setStrategy()` which lets clients replace the strategy associated with the context at runtime.

## Requirements
**Client class**
1. has created an object of the type `ConcreteStrategy`
2. has used the `setStrategy()` in the Context class to store the `ConcreteStrategy` object	
3. has executed the `ConcreteStrategy` via the `Context`-class	

**Context class**
1. has a private field or property of type `Strategy`
2. has a function `setStrategy()` to set the private field/property
    1. if the reference is stored in a field the function must have a parameter of type `Strategy`
3. has a function `useStrategy()` that calls the `execute()` function in `Strategy`

**Strategy interface**
1. is an interface/abstract class	(duplicate if 4 holds and the implementation/inheritance is correct)
2. has declared a method
    1. if the class is an abstract class instead of an interface, this method has to be an abstract method
3. is used by the context class
4. is implemented/inherited by at least one other class
5. is implemented/inherited by at least two other classes

**ConcreteStrategy class**
1. is an implementation of the `Strategy` interface
2. if the class is used, it must be used via the `Context` class
3. if the class is not used it should be used via the `Context` class
4. is stored in the `Context` class

## References
[^1]: Refactoring Guru, Behavioral Patterns - Strategy. https://refactoring.guru/design-patterns/strategy
[^2]: A. Shalloway, J.R. Trott, Design Patterns Explained - A new perspective on Object Oriented Design (p.152 -156). (2004). 
