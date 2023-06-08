# Decorator
## Introduction
The decorator pattern is a structural design pattern that lets you attach new behaviors to objects by 
placing these objects inside special wrapper objects that contain the desired behaviors. The explanation on 
this page is based on the definition of previous development teams, the website _refactoring guru_[^1] 
and the book _Design Patterns Explained_[^2].

### Context of use
The decorator pattern is a way to add additional functionality to an existing function dynamically.
The gang of four describes the decorator pattern in the following manner: “Attach additional responsibilities
to an object dynamically. Decorators provide a flexible alternative to subclassing for extending functionality." 
The decorator pattern comes in handy when there are a variety of optional behaviors that can precede or 
follow another behavior that is always executed.

### Example of use
An example case that shows the need of the pattern is a coffee machine. Different types of condiments can be added to
coffee. When every combination of condiments and coffee is its own class, adding a new condiment would greatly increase
the amount of classes. The decorator pattern is useful in this case since the base coffee can be made the concrete component, and
all the condiments can be made decorators. Now the addition of a new condiment would only require a new decorator class.

## Explanation of Architecture
![UML Decorator](https://refactoring.guru/images/patterns/diagrams/decorator/structure.png)

The **Component** could be an interface or an abstract class. It declares the common interface for both wrappers, and wrapped objects.
It needs to contain a method.

The **ConcreteComponent** is a class defining the basic behavior that can be altered by decorators. 
This class needs to implement `Component`.

The **BaseDecorator** is an abstract class with a field of type `Component`, and a constructor to pass an instance to said field.
It implements the method of `Component` by calling the method of its field.

The **ConcreteDecorator** is a class that inherits from `BaseDecorator`. It implements the method of `Component` by calling its parent's
method, and either before or after calling said method, adding its extra behaviour.

The **Client** can wrap components in multiple layers of decorators, and executes the create object.

## Requirements
The priority of a requirement is noted with the (low)-(mid)-(high)-(knockout) criteria.

**Client class**
1. (low) has created an object of the type `ConcreteComponent`
2. (low) has created an object of the type `ConcreteDecorator`, to which it passes the `ConcreteComponent`
3. (low) has called the method of `ConcreteDecorator`

**Component interface**
1. (knockout) is an interface/abstract class
2. (knockout) has declared a method
    1. if the class is an abstract class instead of an interface, this method has to be an abstract method
3. (knockout) its declared method gets called by the BaseDecorator class 
4. (knockout) is implemented/inherited by at least two other classes

**ConcreteComponent class**
1. (knockout) is an implementation of `Component`
2. (knockout) does not have a field of type `Component`
3. (low) is created by the Client class

**BaseDecorator class**
1. (knockout) is an implementation of `Component`
2. (high) is an abstract class
3. (knockout) has a field of type `Component`
4. (high) has a constructor with a parameter of type `Component`, which it passed to its field
5. (knockout) calls the method of its field in the implementation of the method of `Component`
6. (knockout) is inherited by at least 1 other class

**ConcreteDecorator class**
1. (knockout) inherits from `BaseDecorator`
2. (knockout) calls the method of its parent in the implementation of the method of `Component`
3. (mid) has a function providing extra behavior which it calls in the implementation of the method of `Component`

## References
[^1]: Refactoring Guru, Structural Patterns - Decorator. https://refactoring.guru/design-patterns/decorator
[^2]: A. Shalloway, J.R. Trott, Design Patterns Explained - A new perspective on Object Oriented Design (p.300-310). (2004).
