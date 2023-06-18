# Observer
## Introduction
An observer pattern is a behavioral design pattern that lets you define a subscription mechanism to notify multiple objects about any events that happen to the object they're observing. The explanation on this page is based on the definition of previous development teams, the website _refactoring guru_[^1].

### Context of use
The observer pattern is useful to notify objects about subjects that they can subscribe to. A client class can add subscribers to a publisher which updates the subscribers on certain events. 

### Example of use
A real life example is subscribing to a news paper which will send you new issues when they are published. The interested people do not have to check the with the publisher whether there is a new issue. And the publisher does not have to send out issues to everyone, even the not interested people. 

## Explanation of Architecture
![UML Observer](https://refactoring.guru/images/patterns/diagrams/observer/structure.png)

The **Publisher** issues events of interest to other objects. These events occur when the publisher changes its state or executes some behaviors. Publishers contain a subscription infrastructure that lets new subscribers join and current subscribers leave the list. When a new event happens, the publisher goes over the subscription list and calls the notification method declared in the subscriber interface on each subscriber object.

The **Subscriber** interface declares the notification interface. In most cases, it consists of a single `update` method. The method may have several parameters that let the publisher pass some event details along with the update.

The **Concrete Subscribers** perform some actions in response to notifications issued by the publisher. All of these classes must implement the same interface so the publisher is not coupled to concrete classes. Normally the publisher passes some contextual data as arguments with a notification. The publisher can also pass itself as an argument, so the subscriber can fetch the data itself.

The **Client** creates the publisher and subscriber objects separately and then registers subscribers for publisher updates.

## Requirements
The priority of a requirement is noted with the (low)-(mid)-(high)-(knockout) criteria.

**Subscriber**
1. (knockout) Is an interface or abstract class.
2. (knockout) Has a not private, not internal `update()` method.
    1. (knockout) if `Subscriber` is an abstract class `update()` should be a abstract method.
 
**Publisher**
1. (Knockout) Is a class
2. (Knockout) Has a private field with as type a Subscriber list, `subscribers`
3. (High) Has a private field or property `mainState`
4. (High) Has two public methods that have a parameter with as type `Subscriber` that uses the list `subscribers`
5. (High) Has a public method that uses the `subscribers` list and uses the `update()` method in `Subscriber`
6. (Mid) Has a public mtehod that uses `mainState`

**Concrete Subscriber**
1. (Knockout) is a class that implements the `Subscriber` interface or inherits form the `Subscriber` abstract class
    1. If `Subscriber` is an abstract class it overrides the abstract `update()` method

**Client**
1. (Knockout) Is a class
2. (Mid) Creates the Publisher
3. (Mid) Creates a Concrete Subscriber
4. (Low) Uses a method as described in `Publisher` 4
5. (Low) Uses the method as described in `Publiser` 5

## References
[^1]: Refactoring Guru, Creational Patterns - Observer. https://refactoring.guru/design-patterns/observer
