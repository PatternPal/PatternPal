using System;

namespace IDesign.Tests.TestClasses.BridgeTest2
{
    /// <summary>
    ///     Bridge Design Pattern
    /// </summary>
    public class Client
    {
        public void Method(string[] args)
        {
            // Create RefinedAbstraction
            var customers = new Customers();
            // Set ConcreteImplementor
            customers.Data = new CustomersData("Chicago");
            // Exercise the bridge
            customers.Show();
            customers.Next();
            customers.Show();
            customers.Next();
            customers.Show();
            customers.Add("Henry Velasquez");
            customers.ShowAll();
            // Wait for user
            Console.ReadKey();
        }
    }
}
