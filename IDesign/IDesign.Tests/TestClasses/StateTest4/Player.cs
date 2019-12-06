using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateTest4
{
    // 'Context' class
    public class Player
    {
        State currentState;

        public Player()
        {
            this.currentState = new HealthyState();
        }

        public void Bullethit(int bullets)
        {
            Console.WriteLine("Bullets hits to the player: " + bullets);
            if (bullets < 5)
                this.currentState = new HealthyState();
            if (bullets >= 5 && bullets < 10)
                this.currentState = new HurtState();
            if (bullets >= 10)
                this.currentState = new DeadState();

            currentState.ExecuteCommand(this);
        }
    }
}
