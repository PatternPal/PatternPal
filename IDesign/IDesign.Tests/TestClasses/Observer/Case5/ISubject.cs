namespace IDesign.Tests.TestClasses.Observer.Case5
{
    interface ISubject
    {
        void Add(IObserver observer);
        void Notify();
    }
}
