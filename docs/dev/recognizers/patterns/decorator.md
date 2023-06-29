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
3. (knockout) its declared method gets called by `BaseDecorator` 
4. (knockout) is implemented/inherited by at least two other classes

**ConcreteComponent class**
1. (knockout) is an implementation of `Component`
2. (knockout) does not have a field of type `Component`
3. (knockout) if Component is an abstract class, it overrides the method of Component
4. (low) is created by the `Client` class

**BaseDecorator class**
1. (knockout) is an implementation of `Component`
2. (high) is an abstract class
3. (knockout) has a field of type `Component`
4. (high) the field is private
5. (high) has a non-private constructor with a parameter of type `Component`, which it passes to its field 
6. (knockout) calls the method of its field in the implementation of the method of `Component`
    1. if Component is an abstract class, it overrides the method of Component
7. (knockout) is inherited by at least 1 other class

**ConcreteDecorator class**
1. (knockout) inherits from `BaseDecorator`
2. (knockout) calls the method of its parent in the implementation of the method of `Component`
3. (mid) has a function providing extra behavior which it calls in the implementation of the method of `Component`
4. (low) the function providing extra behaviour does not use the method of Component 

## Step By Step Steps
| Steps | Requirements                                                                                                                                                                                       | Explanations                                                                                                                                                                                                                                                                                                                                                                                              |
|-------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1     | Make an interface with a method. We refer to this interface as `Component`.                                                                                                                        | This will be the common interface for both wrappers (decorators) and wrapped objects (concrete components).                                                                                                                                                                                                                                                                                               |
| 2     | Make a class that implements Component. Also implement the method. We refer to this class as `ConcreteComponent`.                                                                                  | This class defines the basic behavior we want, which, when we are done, can be altered by decorators.                                                                                                                                                                                                                                                                                                     |
| 3     | Make an abstract class that implements Component. Implement the method of Component by making it virtual. Give it a private field with type Component. We refer to this class as `Decorator`.      | This class is abstract, since its only job is delegating operations to the wrapped object, i.e. the field. The field needs to have Component as type, since this class should be able to both wrap concrete components and decorators. It should be private since its only purpose should be to be called in the method. The specific extra behavior will be added by classes inheriting from this class. |
| 4     | Give Decorator a non-private constructor with a parameter that gets assigned to the field.                                                                                                         | This constructor is needed to wrap a component. It should not be private since a class inheriting from this class should be able to pass an instance of Component as a parameter to the constructor.                                                                                                                                                                                                      |
| 5     | In Decorator, call the method of the field in the implementation of the method.                                                                                                                    | This call ensures a decorator will always execute the behavior of its wrapped object.                                                                                                                                                                                                                                                                                                                     |
| 6     | Make a class that inherits from Decorator. Override its method by calling the method of Decorator; base.Method(). We refer to this class as `ConcreteDecorator`.                                   | By calling the method of the parent, we ensure that the behavior of the wrapped object is executed.                                                                                                                                                                                                                                                                                                       |
| 7     | Add a method to ConcreteDecorator providing additional behavior and call it in the overrided method either before or after the call to the parent's method.                                        | Now, the decorator does add additional behavior to the wrapped object.                                                                                                                                                                                                                                                                                                                                    |
| 8     | Make a class with a method that instantiates an instance of ConcreteDecorator by passing to its constructor a new instance of ConcreteComponent. Now call the method of the instantiated variable. | The client can now wrap components in multiple layers of decorators.                                                                                                                                                                                                                                                                                                                                      |

## References
[^1]: Refactoring Guru, Structural Patterns - Decorator. https://refactoring.guru/design-patterns/decorator
[^2]: A. Shalloway, J.R. Trott, Design Patterns Explained - A new perspective on Object Oriented Design (p.300-310). (2004).
