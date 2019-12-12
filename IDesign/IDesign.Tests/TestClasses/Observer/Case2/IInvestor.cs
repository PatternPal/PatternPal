namespace IDesign.Tests.TestClasses.Observer.Case2
{
    //This code is from https://www.dofactory.com/net/observer-design-pattern

    /// <summary>
    /// The 'Observer' interface
    /// </summary>
    interface IInvestor
    {
        void Update(Stock stock);
    }
}
