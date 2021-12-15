namespace IDesign.Tests.TestClasses.StrategyTest1
{
    //this code is from https://github.com/exceptionnotfound/DesignPatterns/blob/master/Strategy/CookMethod.cs

    /// <summary>
    ///     The Strategy abstract class, which defines an interface common to all supported strategy algorithms.
    /// </summary>
    internal abstract class CookStrategy
    {
        public abstract void Cook(string food);
    }
}
