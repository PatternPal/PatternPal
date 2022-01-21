namespace PatternPal.Tests.TestClasses.StateTest3
{
    //this code is from https://www.dofactory.com/net/state-design-pattern

    /// <summary>
    ///     The 'State' abstract class
    /// </summary>
    internal abstract class State
    {
        public abstract void Handle(Context context);
    }
}
