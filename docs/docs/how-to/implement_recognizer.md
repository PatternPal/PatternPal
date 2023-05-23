# Implement a Recognizer

In this guide we will go over implementing a recognizer, covering everything you need to know to
create your own recognizer. You can read more about the available checks
[here](~/docs/technical/recognizer.md), and more about how a recognizer works internally
[here](~/docs/technical/recognizer_runner.md). The recognizer we will be implementing in this guide
is a simplified version of the Singleton recognizer, which contains all the concepts you will
encounter when working with recognizers in PatternPal.

## Prerequisites

Before we get started, make sure you have clear requirements in mind which define the design pattern
you want to recognize. The way in which you have gather these requirements does not really matter,
translation of these requirements into checks is what you will learn in this guide.

## Creating a new Recognizer

To be able to run a recognizer, it needs to implement the `IRecognizer` interface, which defines the
methods required to define a recognizer. The most important of these methods is `Create`, which is
where you will define the checks which your recognizer is made of. After you have created this
class, add the following using statement to the top of the file:

```csharp
using static PatternPal.Core.Checks.CheckBuilder;
```

This static using statement will bring the methods, which serve as a kind of DSL for creating
checks, into scope. After you are done, you should have something like this:

```csharp
using PatternPal.Core.Recognizers;

using static PatternPal.Core.Checks.CheckBuilder;

namespace MyRecognizers;

internal class MyRecognizer : IRecognizer
{
    IEnumerable< ICheck > IRecognizer.Create()
    {
        yield break;
    }
}
```

## Writing your first Check

Now that we have gotten all the setup out of the way, we can get started writing checks. But what is
a check? You can think of a check as a pattern which PatternPal uses as the shape which the analyzed
program should match. In a way, it resembles pattern matching as you might be familiar with from C#
or other languages. However, checks in PatternPal have several features which make it more powerful
than plain pattern matching. More details on these features will follow in later sections.

As we already mentioned, your checks will roughly resemble the structure of the code you analyzing.
There are a couple things to keep in mind while designing your recognizer. First, the root of your
tree of checks must always be a check which matches an entity, so for example an `InterfaceCheck` or
a `ClassCheck`. You should keep in mind that PatternPal starts the recognition from the root of the
code graph, so the first things it encounters are entities. As a method cannot appear outside an
interface or class in C#, this will never be the first thing PatternPal finds. As such, you should
always wrap a `MethodCheck` in either an `InterfaceCheck` or a `ClassCheck`. The only other checks
which may appear as root checks are the so called operator checks. The checks, like `All`, `Any`,
`Not`, and so on, allow you to compose multiple checks, or the check for something which should not
be present in the implementation. These checks are not directly used for pattern matching, so that
is why they can be used as root checks.

### The first Check

The first thing we want to look for when trying to detect Singleton implementations, is a class. So
lets add a check for that:

```csharp
IEnumerable< ICheck > IRecognizer.Create()
{
    yield return Class(
        Priority.Knockout
    );
}
```

Lets go over the things we see in this implementation. The first thing you may notice, is that
`Create` returns an `IEnumerable< ICheck >`, and that the root check is returned using a `yield
return`. The reason you can have multiple root checks, is that there are design patterns which
consist of multiple classes and interfaces. By returning you multiple root checks, you can match all
of those using the same recognizer. In a later section we will also show how to check for relations
between these entities. The second important thing is the priority, which is `Knockout` in this
case. Using priorities you can differentiate between different requirements, and assign an
importance to them. The `Knockout` priority is special in the sense that this marks the check as
required, if PatternPal sees that a check has failed, but it is marked as `Knockout`, it will
immediately disregard the entity being checked from being a possible match.

## Adding nested Checks

Now that you have seen how to create a check, it is time to learn how to add a nested check. A
nested check is a check which is the child of another check. An example of this is a `MethodCheck`
which is the child of a `ClassCheck`.

### Private Constructor Check

To be a Singleton implementation, the class must have a private constructor, so lets add a check
for that.

```csharp
IEnumerable< ICheck > IRecognizer.Create()
{
    yield return Class(
        Priority.Knockout,
        Constructor(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Private
            )
        )
    );
}
```

In this example we immediately see two levels of nesting: the first nested check is the
`ConstructorCheck`, and the second nested check is the `ModifierCheck` inside the
`ConstructorCheck`. This recognizer will now match any class which has a private constructor.

## Operator Checks

As you may have noticed in the previous section, the recognizer does not disallow any non-private
constructors, which is an issue in case of a singleton implementation. To check for the absence of
something, you can use the `NotCheck`.

```csharp
IEnumerable< ICheck > IRecognizer.Create()
{
    yield return Class(
        ...
        Constructor(
            Priority.Knockout,
            Not(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Public
                )
            )
        )
    );
}
```

Because a constructor can have only one modifier, you cannot put the `NotCheck` inside the
`ConstructorCheck` you created in the previous section.

## Referencing the current Entity

Lets take a look at how a check can reference the current entity which is being processed. This is
something we need in our Singleton recognizer, where we want to check if the class has a field in
which the instance of the class is stored.

```csharp
IEnumerable< ICheck > IRecognizer.Create()
{
    yield return Class(
        ...
        Field(
            Priority.Knockout,
            Type(
                Priority.Knockout,
                ICheck.GetCurrentEntity
            )
        )
    );
}
```

The type matched by a `TypeCheck` depends on the check in which it is nested. In case of a
`FieldCheck`, it matches the type of the field. Using `ICheck.GetCurrentEntity` we can match the
current entity which we are trying to match. How this current entity is determined is an
implementation detail of PatternPal, but regardless of how deeply nested you use
`ICheck.GetCurrentEntity`, it will always refer to the entity which you are currently trying to
match.

## Referencing the results of another Check

Now for a more complicated check, lets look at how to use the result of an earlier check. In a
singleton implementation, you need to have a way to access the instance of the Singleton class. This
is usually done using a `GetInstance` method. Aside from being `static`, this method should also use
the field in which the instance is stored. We wrote a check for this field in the previous section,
but we cannot reference this check currently. To access this check, we can store it inside a
variable. Checks are only executed when PatternPal encounters them, so storing a check in a
variable, or defining it in a separate method is perfectly fine, and may lead to more readable
recognizers. So as a first step, store the `FieldCheck` inside a variable and use it inside the
`ClassCheck`.

```csharp
IEnumerable< ICheck > IRecognizer.Create()
{
    FieldCheck instanceFieldCheck = Field(
        Priority.Knockout,
        Type(
            Priority.Knockout,
            ICheck.GetCurrentEntity
        )
    );

    yield return Class(
        Priority.Knockout,
        Constructor(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Private
            )
        ),
        instanceFieldCheck
    );
}
```

Now that we have a way to reference the `FieldCheck`, we can also use its results. In this case, we
want to check that the `GetInstance` method uses the field matched by the `FieldCheck`. To check
this, we can use a `RelationCheck`. `RelationCheck`s are checks which you can use to check for
relations between different entities and nodes. Examples of these relations are `Uses`,
`Implements`, and so on. You can read about all the available relations
[here](~/docs/technical/syntax_graph.md). Combining the `static` modifier check with this
`RelationCheck` check, we get the following `MethodCheck`:

```csharp
IEnumerable< ICheck > IRecognizer.Create()
{
    ...
    yield return Class(
        ...
        instanceFieldCheck,
        Method(
            Priority.Knockout,
            Modifiers(
                Priority.Knockout,
                Modifier.Static
            ),
            Uses(
                Priority.Knockout,
                instanceFieldCheck
            )
        )
    );
}
```

One important thing to note, is that a check whose results are used by another check, should always
appear before the checks which use it. If this order is reversed, the used check will not have any
results yet, and so the using check will fail to find any possible matches.

## Final Code

```csharp
using PatternPal.Core.Recognizers;

using static PatternPal.Core.Checks.CheckBuilder;

namespace MyRecognizers;

internal class MyRecognizer : IRecognizer
{
    IEnumerable< ICheck > IRecognizer.Create()
    {
        FieldCheck instanceFieldCheck = Field(
            Priority.Knockout,
            Type(
                Priority.Knockout,
                ICheck.GetCurrentEntity
            )
        );

        yield return Class(
            Priority.Knockout,
            Constructor(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Private
                )
            ),
            Constructor(
                Priority.Knockout,
                Not(
                    Priority.Knockout,
                    Modifiers(
                        Priority.Knockout,
                        Modifier.Public
                    )
                )
            ),
            instanceFieldCheck,
            Method(
                Priority.Knockout,
                Modifiers(
                    Priority.Knockout,
                    Modifier.Static
                ),
                Uses(
                    Priority.Knockout,
                    instanceFieldCheck
                )
            )
        );
    }
}
```
