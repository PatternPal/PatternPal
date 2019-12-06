using System;

namespace IDesign.Tests.TestClasses.StateTest4
{
    // 'ConcreteStateB' class
    public class HurtState : State
    {
        public void ExecuteCommand(Player player)
        {
            Console.WriteLine("The player is wounded. Please search health points");
        }
    }
}