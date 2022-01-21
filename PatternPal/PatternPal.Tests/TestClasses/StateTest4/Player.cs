using System;

namespace PatternPal.Tests.TestClasses.StateTest4
{
    //this code is from http://gyanendushekhar.com/2016/11/05/state-design-pattern-c/

    // 'Context' class
    public class Player
    {
        private IState currentState;

        public Player()
        {
            currentState = new HealthyState();
        }

        public void Bullethit(int bullets)
        {
            Console.WriteLine("Bullets hits to the player: " + bullets);
            if (bullets < 5)
            {
                currentState = new HealthyState();
            }

            if (bullets >= 5 && bullets < 10)
            {
                currentState = new HurtState();
            }

            if (bullets >= 10)
            {
                currentState = new DeadState();
            }

            currentState.ExecuteCommand(this);
        }
    }
}
