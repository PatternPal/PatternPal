# Strategy
## Introduction

### Context of use
The strategy pattern comes in handy when multiple methods do the same thing in different ways, this pattern is in this case used to reduce code duplication. The gang of four describes the Strategy Pattern in the following manner: “Define a family of algorithms, encapsulate each one, and make them interchangeable. Strategy lets the algorithm vary independently from clients that use it”. It enables the user to use different business rules or algorithms depending on the context in which they occur.

### Example of use
An example case that shows the need of the pattern is when someone uses a route calculator which calculates the shortest route from A to B. Someone then might want to use the strategy pattern for each of the different routing options (Bike, Car, etc). Then the code can just call a function CreateRoute independent of which routing algorithm is selected.

## Explanation of Architecture
![UML Strategy](https://refactoring.guru/images/patterns/diagrams/strategy/structure.png)

The **Context** is the class from where the code gets executed. It stores a reference to one of the concrete strategies. It only communicates with the object via the methods in the Strategy.

The **Strategy** could be an interface or an abstract class. This instance needs to contain a (abstract) method that will be used to execute the algorithm as implemented in the ConcreteStrategie classes.

The **ConcreteStrategy** class is designed for one specific purpose and implements/inherits the interface Strategy. This class implements the execute() function and with that defines the algorithm in its own way.

The **Client** creates a ConcreteStrategy object and passes it to the Context. The Context exposes a setter setStrategy() which lets clients replace the strategy associated with the context at runtime.

## Requirements
_Client class_
a. has created an object of the type ConcreteStrategy
b. has used the setStrategy() in the Context class to store the ConcreteStrategy object	
c. has executed the ConcreteStrategy via the Context class	

_Context class_
a. has a private field or property that has the Strategy class as type 
b. has a function setStrategy() to set the private field / property
  i. if the reference is stored in a field the function must have a parameter of type Strategy
c. has a function useStrategy() that calls the execute() function in Strategy.

_Strategy interface_
a. is an interface / abstract class	(duplicate if 4.d holds and the implementation / inheritance is correct)
b. has declared a method
  i. if the class is an abstract class instead of an interface, this method has to be an abstract method.
c. is used by the context class
d. is implemented / inherited by at least one other class
e. is implemented / inherited by at least two other classes

_ConcreteStrategy class_
a. is an implementation of the Strategy interface
b. if the class is used, it must be used via the context class
c. if the class is not used it should be used via the context class
d. is stored in the context class


## References
The following sources are used to determine the requirements:
1. Refactoring Guru, Behavioral Patterns - Strategy. https://refactoring.guru/design-patterns/strategy
2. A. Shalloway, J.R. Trott, Design Patterns Explained - A new perspective on Object Oriented Design (p.152 -156). (2004). 
