using System;

namespace IDesign.Tests.TestClasses.StateTest4
{
    // 'ConcreteStateC' class
    public class DeadState : State
    {
        public void ExecuteCommand(Player player)
        {
            Console.WriteLine("The player is dead. Game Over.");
        }
    }
}