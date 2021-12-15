namespace IDesign.Tests.TestClasses.StrategyTest3
{
    //code is from https://www.dofactory.com/net/strategy-design-pattern

    /// <summary>
    ///     The 'Context' class
    /// </summary>
    internal class Context
    {
        private readonly Strategy _strategy;

        // Constructor
        public Context(Strategy strategy)
        {
            _strategy = strategy;
        }

        public void ContextInterface()
        {
            _strategy.AlgorithmInterface();
        }
    }
}
