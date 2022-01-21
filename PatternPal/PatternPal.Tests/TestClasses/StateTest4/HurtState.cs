using System;

namespace PatternPal.Tests.TestClasses.StateTest4
{
    //this code is from http://gyanendushekhar.com/2016/11/05/state-design-pattern-c/

    // 'ConcreteStateB' class
    public class HurtState : IState
    {
        public void ExecuteCommand(Player player)
        {
            Console.WriteLine("The player is wounded. Please search health points");
        }
    }
}
