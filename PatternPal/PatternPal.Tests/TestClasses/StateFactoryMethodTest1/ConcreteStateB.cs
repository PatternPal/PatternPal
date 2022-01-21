namespace PatternPal.Tests.TestClasses.StateFactoryMethodTest1
{
    internal class ConcreteStateB : IState
    {
        public void Handle(DubbleFactory context)
        {
            context.State = new ConcreteStateA();
        }
    }
}
