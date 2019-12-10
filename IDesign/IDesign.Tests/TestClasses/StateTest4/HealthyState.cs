using System;

namespace IDesign.Tests.TestClasses.StateTest4
{
    //this code is from http://gyanendushekhar.com/2016/11/05/state-design-pattern-c/

    // 'ConcreteStateA' class
    public class HealthyState : State
    {
        public void ExecuteCommand(Player player)
        {
            Console.WriteLine("The player is in Healthy State.");
        }
    }
}