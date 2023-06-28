namespace PatternPal.Tests.New_TestCasesRecognizers.Observer;
//This test is a possible "perfect" observer implementation.
/* Pattern:              Adapter
 * Original code source: https://www.dotnettricks.com/learn/designpatterns/adapter-design-pattern-dotnet
 *
 *
 * Requirements to fullfill the pattern:
 *         Observer Interface
 *            ✓  a) is an interface
 *            ✓  b) has a public or internal 'update()' method
 *         Concrete Observer
 *            ✓  a) is a class that implements the `Observer` interface
 *          Subject Interface
 *            ✓  a) is an interface
 *            ✓  b) has a public or internal method that has a parameter with as type `Observer`
 *          Concrete Subject
 *            ✓  a) is a class that implements the `Subject` interface 
 *            ✓  b) has a private field with as type a `Observer` list, `observers` 
 *            ✓  c) has a public or internal methods that has a parameter with as type `Observer` that uses the list `observers`
 *            ✓  d) has a public method that uses the `observers` list and uses the `update()` method in `Observer`
 *            ✓  e) has either,
 *                   1. both,
 *                       1. a private or protected field or property `mainState`.
 *                       2. has a public or internal method that uses `mainState`.
 *                   2. a public or internal method with a parameter.
 *          Client
 *            ✓  a) creates the Subject
 *            ✓  b) creates a Concrete Observer
 *            ✓  c) uses a method as described in `Concrete Subject` c)
 *            ✓  d) uses the method as described in `Concrete Subject` d)
 */

internal class ObserverTestCase01
{
    interface Observer
    {
        public void Update();
    }

    class ConcreteObserver : Observer
    {
        public void Update()
        {
            Console.WriteLine("We have been updated");
        }
    }

    interface Subject
    {
        public void Attach(Observer observer);
        public void Notify();
    }

    class ConcreteSubject : Subject
    {
        private List<Observer> observers = new List<Observer>();
        private int mainState;

        public void Attach(Observer observer)
        {
            observers.Add(observer);
        }

        public void Notify()
        {
            foreach (Observer observer in observers)
            {
                observer.Update();
            }
        }

        public void ChangeState(int state)
        {
            mainState = state;
            Notify();
        }
    }

    class ClientClass
    {
        public ClientClass()
        {
            Subject subject = new ConcreteSubject();
            Observer observer = new ConcreteObserver();

            subject.Attach(observer);
            subject.Notify();
        }
    }

}
