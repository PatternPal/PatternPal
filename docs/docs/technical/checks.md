# Checks
There are multiple categories of checks which will be explained below. The categories are: leaf checks, node checks and checkCollections.

## Leaf Checks
A leaf check examines a single aspect of an entity (see
the [syntax graph explanation](syntax_graph.md)) and compares it to a specific value. They do not have child checks therefore their result also does not hold childcheck results. The leaf checks that are now implemented are explained below.

### ModifierCheck

A modifierCheck examines the modifiers of an entity and compares them to the ones specified in the check. The modifiers that can be checked are public, internal, protected, private, abstract, const, extern, override, partial, readonly, sealed, static, unsafe, virtual, volatile, new and async.

### TypeCheck

A type check verifies that the type of an entity matches a specific
type, as specified in the check. For instance,
in the Singleton design pattern, a type check could verify that the
return type of the `GetInstance()` method is the same as the type of the
class which contains the method.

### ParameterCheck

A parameter check examines the parameters of an entity like a method or
constructor and compares them to the expected ones in the design pattern
being recognized. For instance, in the Strategy design pattern, a
parameter check could verify that the `SetStrategy()` method of the `Context`
class has a parameter of the `StrategyInterface` type. This is done with a list of
TypeChecks.

### RelationCheck

A relation check examines a specific relation between two entities in
the code and compares it to the expected relationships in the design
pattern being recognized. Different types of relations that can be
checked are usage, creation, inheritance, and implementation. For
example, a check for the Adapter design pattern would include a relation
check confirming that the `Adapter` class uses the `Adaptee` class.

## Node Checks

A Node check examines an entity. It combines Node and Leaf checks into a
more complex check for the entity being checked.

### NodeCheck

A NodeCheck is a class encompassing the behavior of a node check, i.e.
the possibility to run a collection of subchecks. It is the base of all
node checks. It achieves this with the use of a template method
implementation of the Check() function. For each check where different
implementations are possible, this function calls a virtual method that
can be implemented by the specific node checks. For example, what the
type of a node represents differs per node. The type of a method is its
return type, while the type of a constructor is its parent type. Thus
there is a virtual function that gets the type of field and is
implemented in the specific node checks, and said function is used in
the `Check()` function in the case of a subcheck of type `TypeCheck`.

### FieldCheck

A field is a globally defined variable. A FieldCheck examines whether
there is a certain field in the entity being examined that complies with
the child checks of the FieldCheck. A FieldCheck can compose TypeChecks,
ModifierChecks, and operator checks.

### MethodCheck

A MethodCheck examines whether there is a certain method in the entity
being examined that complies with the child checks of the MethodCheck. A
MethodCheck can compose ModifierChecks, ParameterChecks, RelationChecks,
TypeChecks, and operator checks. The Typechecks examine the return type
of the method.

### ConstructorCheck

A ConstructorCheck examines whether there is a certain constructor in
the entity being examined that complies with the child checks of the
ConstructorCheck. A ConstructorCheck can compose ModifierChecks,
ParameterChecks, RelationChecks, TypeChecks, and operator checks. The
TypeChecks examine the return type of the constructor, which is equal to
the type of the entity that composes it.

### PropertyCheck

A PropertyCheck examines whether there is a certain property in the
entity being examined that complies with the child checks of the
PropertyCheck. A PropertyCheck can compose ModifierChecks,
RelationChecks, TypeChecks, and operator checks. The Typechecks examine
the return type of the property.

### ClassCheck

A ClassCheck examines whether there is a certain class that complies
with the checks of the ClassCheck. A ClassCheck can compose
MethodChecks, PropertyChecks, ConstructorChecks, ModifierChecks,
RelationChecks, and operator checks.

### InterfaceCheck

An InterfaceCheck examines whether there is a certain interface that
complies with the checks of the InterfaceCheck. An InterfaceCheck can
compose MethodChecks, PropertyChecks, ModifierChecks, RelationChecks,
and operator checks.

### Operator Checks

Some checks are similar to logical operators. They combine the result of
their collection of checks in a way similar to how logical operators
combine boolean values. Currently, the only implemented operator check
is the NotCheck.

**NotCheck**
NotCheck contains a check and negates its result. This can be used for
example when instead of requiring a specific modifier, you exclude one.

## CheckCollection
CheckCollection can be used to check the amount of childchecks that must return correctly. 
For now this can be an 'Any' checkCollection, which is correct when one of the childs are correct. 
And the 'All' checkCollection, which is correct when all the childs are correct.

## Creating a new Check
1. Create a ...Check.cs class in the `PatternPal.Core/Checks` folder 
2. Decide whether the check is a nodeCheck, i.o.w. if the check can have childChecks
  _ If the check is a nodeCheck implement the `NodeCheck< TNode >`
  _ otherwise, implement the `Checkbase`
3. Add the check to the switch in the `NodeCheck< TNode >.RunCheck()` for a derivation from the `NodeCheck< INode >` one should probably add some extra logic. Otherwise this is done in the override of `Check()`.

4. Create a ...CheckTest.cs file in the `PatternPal.Tests/Checks` folder and check the functionality. There are some helpfull methods in `PatternPal.Tests/Utils`.