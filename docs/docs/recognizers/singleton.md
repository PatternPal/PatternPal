# Singleton
## Introduction

### Context of use
The singleton pattern is useful when you want to ensure there is only one instance of a class, while also providing global access to this class. In addition to this you want to ensure that all entities are using the same instance of this object, without passing a reference to all of them.

### Example of use
An example of use could be some sort of database. There should be global access to this instance but it is not preferred that there are multiple databases that all hold part of the information.

## Explanation of Architecture
![UML Singleton](https://refactoring.guru/images/patterns/diagrams/singleton/structure-en-indexed.png?id=b0217ae066cd3b757677d119551f9a8f)

The **singleton** class declares the static method getInstance() that returns an instance of its own class. The constructor of the class should be hidden from the client code and should only be called the first time the getInstance() method is called. All other times it returns this instance.

## Requirements
_Client Class_
a. calls the method that acts as a constructor of the singleton class

_Singleton class_ 
a. has no public/internal constructor
b. has at least one private/protected constructor
c. has a static, private field with the same type as the class
d. has a static, public/internal method that acts as a constructor in the following way:
  i. if called and there is no instance saved in the private field, then it calls the private constructor
  ii. if called and there is an instance saved in the private field it returns that instance

## References
The following sources are used to determine the requirements:
1. Refactoring Guru, Creational Patterns - Singleton. https://refactoring.guru/design-patterns/singleton
2. A.Shalloway, J.R.Trott, Design Patterns Explained - A new perspective on Object Oriented Design (p.363). (2004)
