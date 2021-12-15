namespace IDesign.Tests.TestClasses.ObserverTest2
{
    //This code is from https://www.dofactory.com/net/observer-design-pattern

    /// <summary>
    ///     The 'Observer' interface
    /// </summary>
    internal interface IInvestor
    {
        void Update(Stock stock);
    }
}
