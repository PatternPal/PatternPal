namespace PatternPal.Tests.TestClasses.ObserverTest3
{
    internal class ObserverA : IObserver
    {
        public void Update()
        {
            Console.WriteLine("very nice");
        }
    }
}
