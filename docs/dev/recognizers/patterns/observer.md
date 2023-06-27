# Observer
## Introduction
An observer pattern is a behavioral design pattern that lets you define a subscription mechanism to notify multiple objects about any events that happen to the object they're observing. This patterns is sometimes refered to as Event-Subscriber or Listener.  The explanation on this page is based on the definition of previous development teams and the book Design patterns explained[^1].

### Context of use
The observer pattern is useful to notify objects about subjects that they can subscribe to. A client class can add observers to a subject which updates the observers on certain events. 

### Example of use
A real life example is subscribing to a news paper which will send you new issues when they are published. The interested people do not have to check the with the publisher whether there is a new issue. And the publisher does not have to send out issues to everyone, even the not interested people. 

## Explanation of Architecture
![UML Observer]('../../../../../images/ObserverUML.png')

The **Subject** is the interface for the `ConcreteSubjects` it consists of a `Attach()`, `Detach()` and `Notify()` methods which should be implemented in the `ConcreteSubjects`. 

The **Concrete Subject** class implements the `Subject` interface and it's methods. It holds a list of all the `Concrete Observers`, this list can be altered by using the `Attach()` and `Detach()` methods. The `Notify()` method is used to update all the `Concrete Observer` instances. In addition to this can it store a state field. When this state value changes it is meant to notify the `Concrete Observer` instances. A state value can also be send via a method in the class. 

The **Observer** interface declares the notification interface. In most cases, it consists of a single `update` method. The method may have several parameters that let the subject pass some event details along with the update.

The **Concrete Observer** perform some actions in response to notifications issued by the subject. All of these classes must implement the same interface so the subject is not coupled to concrete classes. Normally the subject passes some contextual data as arguments with a notification. The subject can also pass itself as an argument, so the observer can fetch the data itself. Another possibility is keeping a observer state, in this way it can get the information itself whenever it wants.

The **Client** creates the publisher and observer objects separately and then registers observers for publisher updates.

## Requirements
The priority of a requirement is noted with the (low)-(mid)-(high)-(knockout) criteria.

**Observer**
1. (Knockout) Is an interface.
2. (Knockout) Has a `update()` method.

**Concrete Observer**
1. (Knockout) Is a class that implements the `Observer` interface.

**Subject** 
1. (Knockout) Is an interface
2. (Knockout) Has a public or internal method that has a parameter with as type `Observer`.

**Concrete Subject**
1. (Knockout) Is a class that implements the `Subject` interface.
2. (Knockout) Has a private field with as type a Observer list, `observers`.
3. (Knockout) Has a public or internal method that has a parameter with as type `Observer` that uses the list `observers`.
4. (Knockout) Has a public method that uses the `observers` list and uses the `update()` method in `Observer`.
5. (High) Has either,
    1. (High) Both,
        1. (High) A private or protected field or property `mainState`.
        2. (High) Has a public or internal method that uses `mainState`.
    2. (High) A public / internal method with a parameter.

**Client**
1. (Mid) Creates the Subject.
2. (Mid) Creates a Concrete Observer.
3. (Low) Uses a method as described in `Concrete Subject` 3.
4. (Low) Uses the method as described in `Concrete SubjecThi` 4.



## References
[^1]: A.Shalloway, J.R.Trott, Design Patterns Explained - A new perspective on Object Oriented Design (p.270-275). (2001)
