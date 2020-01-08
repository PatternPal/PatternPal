namespace IDesign.Tests.TestClasses.ObserverTest5
{
    interface ISubject
    {
        void Add(IObserver observer);
        void Notify();
    }
}
