namespace PatternPal.Tests.TestClasses.StrategyTest2
{
    //this code is from https://refactoring.guru/design-patterns/strategy/csharp/example

    // The Strategy interface declares operations common to all supported
    // versions of some algorithm.
    //
    // The Context uses this interface to call the algorithm defined by Concrete
    // Strategies.
    public interface IStrategy
    {
        object DoAlgorithm(object data);
    }
}
