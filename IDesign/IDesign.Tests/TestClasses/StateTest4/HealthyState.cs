using System;

namespace IDesign.Tests.TestClasses.StateTest4
{
    // 'ConcreteStateA' class
    public class HealthyState : State
    {
        public void ExecuteCommand(Player player)
        {
            Console.WriteLine("The player is in Healthy State.");
        }
    }
}