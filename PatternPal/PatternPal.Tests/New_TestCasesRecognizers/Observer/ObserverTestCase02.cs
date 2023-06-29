namespace PatternPal.Tests.New_TestCasesRecognizers.Observer;
//This test is a possible "perfect" observer implementation.
/* Pattern:              Adapter
 * Original code source: https://www.dotnettricks.com/learn/designpatterns/adapter-design-pattern-dotnet
 *
 *
 * Requirements to fullfill the pattern:
 *         Observer Interface
 *               a) is an interface
 *            ✓  b) has a public or internal 'update()' method
 *         Concrete Observer
 *               a) is a class that implements the `Observer` interface
 *          Subject Interface
 *            ✓  a) is an interface
 *            ✓  b) has a public or internal method that has a parameter with as type `Observer`
 *          Concrete Subject
 *            ✓  a) is a class that implements the `Subject` interface 
 *            ✓  b) has a private field with as type a `Observer` list, `observers` 
 *            ✓  c) has a public or internal methods that has a parameter with as type `Observer` that uses the list `observers`
 *               d) has a public method that uses the `observers` list and uses the `update()` method in `Observer`
 *               e) has either,
 *                   1. both,
 *                       1. a private or protected field or property `mainState`.
 *                       2. has a public or internal method that uses `mainState`.
 *                   2. a public or internal method with a parameter.
 *          Client
 *            ✓  a) creates the Subject
 *               b) creates a Concrete Observer
 *               c) uses a method as described in `Concrete Subject` c)
 *               d) uses the method as described in `Concrete Subject` d)
 */

abstract class IObserver2
{
    public abstract void Update();
}

class ConcreteObserver2 : IObserver2
{
    public override void Update()
    {
        Console.WriteLine("We have been updated");
    }
}

interface ISubject2
{
    public void Attach(Observer observer);
}

class ConcreteSubject2 : ISubject2
{
    private List<Observer> observers = new List<Observer>();

    public void Attach(Observer observer)
    {
        observers.Add(observer);
    }
}

class ClientClass2
{
    public ClientClass2()
    {
        Subject subject = new ConcreteSubject();
    }
}

