using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.TestClasses.StateFactoryMethodTest1
{
    //Context class
     class DubbleFactory : BeerFactory
    {
        public override IBeer BrewBier()
        {
            return new Dubbel();
        }

        private IState _state;

        // Constructor
        public DubbleFactory(IState state)
        {
            this.State = state;
        }

        // Gets or sets the state
        public IState State
        {
            get { return _state; }
            set

            {
                _state = value;
                Console.WriteLine("State: " +
                  _state.GetType().Name);
            }
        }

        public void Request()
        {
            _state.Handle(this);
        }
    }
}
