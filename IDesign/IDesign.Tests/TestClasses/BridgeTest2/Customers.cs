using System;

namespace IDesign.Tests.TestClasses.BridgeTest2
{
    /// <summary>
    ///     The 'RefinedAbstraction' class
    /// </summary>
    public class Customers : CustomersBase
    {
        public override void ShowAll()
        {
            // Add separator lines
            Console.WriteLine();
            Console.WriteLine("------------------------");
            base.ShowAll();
            Console.WriteLine("------------------------");
        }
    }
}
