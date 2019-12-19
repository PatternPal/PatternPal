using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateTest4
{
    //this code is from http://gyanendushekhar.com/2016/11/05/state-design-pattern-c/

    // 'Context' class
    public class Player
    {
        IState currentState;

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
