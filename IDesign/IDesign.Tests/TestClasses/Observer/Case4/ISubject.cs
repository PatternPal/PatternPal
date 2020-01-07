namespace IDesign.Tests.TestClasses.Observer.Case4
{
    interface ISubject
    {
        void Add(IObserver observer);
        void Remove(IObserver observer);
        void Notify();
    }
}
