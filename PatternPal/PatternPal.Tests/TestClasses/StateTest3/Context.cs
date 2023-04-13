namespace PatternPal.Tests.TestClasses.StateTest3
{
    //this code is from https://www.dofactory.com/net/state-design-pattern

    /// <summary>
    ///     The 'Context' class
    /// </summary>
    internal class Context
    {
        private State _state;

        // Constructor
        public Context(State state)
        {
            State = state;
        }

        // Gets or sets the state
        public State State
        {
            get => _state;
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
