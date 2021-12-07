namespace IDesign.Tests.TestClasses.ObserverTest4
{
    internal interface ISubject
    {
        void Add(IObserver observer);
        void Remove(IObserver observer);
        void Notify();
    }
}
