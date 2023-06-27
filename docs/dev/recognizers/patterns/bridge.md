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
3. (low) sets the field or property in `Abstraction`, either through
    1. a constructor in `Abstraction` with a parameter of type `Implementation`
    2. a method in `Abstraction` with a parameter of type `Implementation` 
    3. setting the property

**Abstraction class**
1. (knockout) has a private / protected field or property with the `Implementation` type
2. (knockout) has a method 
3. (high) there is a method that calls a method in `Implementation`
4. (mid) has either
    1. the property option as described in 1.
    2. a constructor with a parameter with the `Implementation` type and that uses the field as described in 1.
    3. a method with a parameter with the `Impelementation` type and that uses the field as described in 1.

**Implementation**
1. (knockout) is an interface or abstract class
2. (high) has at least one (abstract) method

**Concrete Implementations**
1. (knockout) is an implementation of the `Implementation` interface or inherits from the `Implementation` abstract class
2. (high) if `Implementation` is an abstract class it should override it's abstract methods

**Refined Abstraction**
1. (knockout) inherits from the `Abstraction` class
2. (high) has an method

## Step By Step Steps
| Steps | Requirements                                                                                                                                                                                                                                            | Explanations                                                                                                                                                                                                                |
|-------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1     | Make an interface or abstract class with a (if possible: abstract) method. I will refer to this as the Implementation Interface or Abstract Class.                                                                                                      | This is the interface or abstract class that is the base for concrete implementation classes.                                                                                                                               |
| 2     | Make a class with a private or protected field or property with an Implementation type. I will refer to this class as the Abstraction Class.                                                                                                            | This class can provide a high-level control logic. It relies on the implementation object to do the actual low-level work. It stores a Concrete Implementation instance in the property or field.                           |
| 3     | Make a method in the Abstraction class that calls the method in the Implementation Interface or Abstract Class.                                                                                                                                         | This method is used to execute the actual low-level work that is defined in a Concrete Implementation instance. It only communicates with a Concrete Implementation through the Implementation Interface or Abstract Class. |
| 4     | If you chose to create a field in step 2, you should create a constructor or method with a parameter with the Implementation type that sets the field of step 2 to the value of the parameter.                                                          | It is time to make it possible that the field or property you created in step 2 has a value. If you created a property this will be enough, if you made a field you should make a constructor or method to do this.         |
| 5     | Make a class that implements the Implementation Interface or inherits form the Implementation Abstract Class. If it inherits from the Abstract Class it should override the abstract method. I will refer to this as the Concrete Implementation Class. | This is a Concrete Implementation Class in which you can define the low-level work that an instance of the Abstraction Class can use.                                                                                       |
| 6     | Make a class that inherits from the Abstraction class and has a method. This class is called the Refined Abstraction Class.                                                                                                                             | This class can be used to define some more specific high-level work.                                                                                                                                                        |
| 7     | Make a class that uses a method in the Abstraction Class. I will refer to this class as the Client Class.                                                                                                                                               | This Client Class is used to control and use the Bridge pattern.                                                                                                                                                            |
| 8     | Let the Client Class create a Concrete Implementation instance and pass it through either a property, constructor or method to the Abstraction class.                                                                                                   | In order to complete the use of the Bridge pattern you should create a Concrete Implementation that the Abstraction class will use to execute the work.                                                                     |

## References
[^1]: Refactoring Guru, Creational Patterns - Bridge. https://refactoring.guru/design-patterns/bridge

