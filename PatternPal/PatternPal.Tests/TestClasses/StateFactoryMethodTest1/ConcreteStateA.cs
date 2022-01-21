namespace PatternPal.Tests.TestClasses.StateFactoryMethodTest1
{
    internal class ConcreteStateA : IState
    {
        public void Handle(DubbleFactory context)
        {
            context.State = new ConcreteStateB();
        }
    }
}
