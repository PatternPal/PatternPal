# Functioning of the recognizers

The [appendix](#appendix) can be used as a tool accompanying the reading of this section.

## What is a recognizer

A recognizer tries to detect a specific design pattern in the
programmer's code. Such a recognizer comprises a collection of checks
examining the static code and identifying the critical parts of the
detected design pattern. These recognizers are created by dynamically
composing modular checks using the fluent design pattern.

## Leaf Checks

Leaf checks examine a single aspect of an entity (see
JEROEN) and compare it to a specific value. They do
not contain other checks.

### ModifierCheck

A modifier check examines the modifiers (public, private, static,
abstract, etc.) of an entity and compares them to the ones specified in
the check. For instance, a modifier check for the Singleton design
pattern would include a check for a private constructor.

### TypeCheck

A type check verifies that the type of an entity matches a specific
type, as defined by the design pattern being recognized. For instance,
in the Singleton design pattern, a type check could verify that the
return type of the GetInstance method is the same as the type of the
class which contains it.

### ParameterCheck

A parameter check examines the parameters of an entity like a method or
constructor and compares them to the expected ones in the design pattern
being recognized. For instance, in the Strategy design pattern, a
parameter check could verify that the SetStrategy method of the Context
class has a parameter of type Strategy. This is done with a list of
TypeChecks.

### RelationCheck

A relation check examines a specific relation between two entities in
the code and compares it to the expected relationships in the design
pattern being recognized. Different types of relations that can be
checked are usage, creation, inheritance, and implementation. For
example, a check for the Adapter design pattern would include a relation
check confirming that the Adapter class uses the Adaptee class.

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
the Check() function in the case of a subcheck of type TypeCheck.

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

## Operator Checks

Some checks are similar to logical operators. They combine the result of
their collection of checks in a way similar to how logical operators
combine boolean values. Currently, the only implemented operator check
is NotCheck.

### NotCheck

NotCheck contains a check and negates its result. This can be used for
example when instead of requiring a specific modifier, you exclude one.

## Priority

Each check has a priority. This indicates the importance of the
characteristic the check validates being present in the design pattern.
There are four levels of priority, Knockout, High, Mid, and Low. A check
with a Knockout priority must succeed for the design pattern to be
considered at all. A certain predetermined percentage of the High checks
must succeed as well for the pattern to be considered. If this
percentage is met, but not all high checks are met, then these
unfulfilled checks are either critical points of improvement or an
indication the pattern is not implemented after all. Mid and Low checks
do not serve as determinants for the consideration of a pattern but
rather function as qualifiers for assessing the level of proficiency in
the implementation of the design pattern. They are used as final pattern
qualifiers and serve as points for further improvement.

## CheckResult

A CheckResult is the result of a check. Similar to how a check can be a
collection of checks, the result of a check can be a collection of
CheckResults. Each CheckResult has a feedback message, which provides
information about what happened when the check was run. For example, it
shows for what reason a check did not succeed.\
A leaf check corresponds to a LeafCheckResult. Such a CheckResult either
did succeed or did not. Therefore, it has a Correctness property. A node
check corresponds to a NodeCheckResult. It has a list of
NodeCheckResults and LeafCheckResults.

## CheckBuilder

The CheckBuilder is a static class that makes it possible to make a
collection of checks for a recognizer. It contains a number of static
methods, each one for one specific check, like Class() which returns a
ClassCheck, and Method() which returns a MethodCheck. These methods can
be used by a recognizer to create checks in a fluid way.

### Example recognizer

This recognizer checks whether there is a class which has a static,
non-private method which is used by one of its other methods.

## Running a recognizer

The recognizers are run on the code base of the user in the Run()
function of the RecognizerRunner.\
The Runner creates a SyntaxGraph of the code base. Here the code base
gets represented as a graph of entities (see
JEROEN. It also creates a RecognizerContext which
gets a reference to this graph. Then it goes through each recognizer and
gives each entity of the graph in turn, as root entity, together with
the context, to the recognizer. The recognizer then runs all of its
checks based on this root entity as the entry point to the code base. In
other words, the root check of the recognizer will correspond to the
root entity given.

## Result of a recognizer

Currently, a recognizer generates a list of all potential linkages
between the entities in the graph and the checks from the recognizer's
checklist. This is done in such a way that leaf checks pass and the
structure of the checklist is maintained. What is meant by structure is
the following: if a recognizer contains a method check in a class check,
the identified entity linked to the class check should include the
identified entity linked to the method check.\
Filtering based on priorities is not yet implemented.

## Appendix

![easter egg](images/recognizer_uml.png "A generalized UML diagram of the recognizers.")
