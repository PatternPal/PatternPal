namespace PatternPal.Tests.TestClasses.StateFactoryMethodTest1
{
    //Context class
    internal class DubbleFactory : BeerFactory
    {
        private IState _state;

        // Constructor
        public DubbleFactory(IState state)
        {
            State = state;
        }

        // Gets or sets the state
        public IState State
        {
            get => _state;
            set

            {
                _state = value;
                Console.WriteLine("State: " +
                                  _state.GetType().Name);
            }
        }

        public override IBeer BrewBier()
        {
            return new Dubbel();
        }

        public void Request()
        {
            _state.Handle(this);
        }
    }
}
