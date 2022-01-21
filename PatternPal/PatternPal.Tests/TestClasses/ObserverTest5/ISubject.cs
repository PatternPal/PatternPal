namespace PatternPal.Tests.TestClasses.ObserverTest5
{
    internal interface ISubject
    {
        void Add(IObserver observer);
        void Notify();
    }
}
