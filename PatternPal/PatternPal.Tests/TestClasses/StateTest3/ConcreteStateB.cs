namespace PatternPal.Tests.TestClasses.StateTest3
{
    //this code is from https://www.dofactory.com/net/state-design-pattern

    /// <summary>
    ///     A 'ConcreteState' class
    /// </summary>
    internal class ConcreteStateB : State
    {
        public override void Handle(Context context)
        {
            context.State = new ConcreteStateA();
        }
    }
}
