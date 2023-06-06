# Singleton
## Introduction
A singleton pattern is a creational design pattern that lets you ensure that a class has only one instance, while providing a global access point to this instance. The explanation on this page is based on the definition of previous development teams, the website _refactoring guru_[^1] and the book _Design Patterns Explained_[^2].

### Context of use
The singleton pattern is useful when you want to ensure there is only one instance of a class, while also providing global access to this class. In addition to this you want to ensure that all entities are using the same instance of this object, without passing a reference to all of them.

### Example of use
An example of use could be some sort of database. There should be global access to this instance but it is not preferred that there are multiple databases that all hold part of the information.

## Explanation of Architecture
![UML Singleton](https://refactoring.guru/images/patterns/diagrams/singleton/structure-en-indexed.png?id=b0217ae066cd3b757677d119551f9a8f)

The **singleton** class declares the static method `getInstance()` that returns an instance of its own class. The constructor of the class should be hidden from the client code and should only be called the first time the `getInstance()` method is called. All other times it returns the previously created instance.

## Requirements
The priority of a requirement is noted with the (low)-(mid)-(high)-(knockout)

**Client class**
1. (mid) calls the method that acts as a constructor of the singleton class

**Singleton class**
1. (knockout) has no public/internal constructor
2. (knockout) has at least one private/protected constructor
3. (knockout) has a static, private field with the same type as the class
4. (knockout) has a static, public/internal method that acts as a constructor in the following way:
    1. (mid) if called and there is no instance saved in the private field, then it calls the private constructor
    2. (mid) if called and there is an instance saved in the private field it returns that instance

## References
[^1]: Refactoring Guru, Creational Patterns - Singleton. https://refactoring.guru/design-patterns/singleton
[^2]: A.Shalloway, J.R.Trott, Design Patterns Explained - A new perspective on Object Oriented Design (p.363). (2004)
