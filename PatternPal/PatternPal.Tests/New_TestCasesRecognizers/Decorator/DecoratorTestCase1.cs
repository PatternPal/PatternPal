namespace PatternPal.Tests.New_TestCasesRecognizers.Decorator;

/* Pattern:              Decorator
     * Original code source: None
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Component interface
     *            ✓  a) is an interface	/ abstract class
     *            ✓  b) has declared a method
     *            ✓        1) if the class is an abstract instead of an interface the method has to be an abstract method
     *            ✓  c) is implemented / inherited by at least two other classes
     *         Concrete Component
     *            ✓  a) is an implementation of the Component interface
     *         Base Decorator
     *            ✓  a) is an abstract class
     *            ✓  b) has a field of type Component
     *            ✓  c) has a constructor with a parameter of type Component, which it passes to its field
     *            ✓  d) calls the method of its field in the implementation of the method of Component
     *            ✓  e) is an implementation of the Component interface TODO toevoegen aan docs
     *         Concrete Decorator
     *            ✓  a) inherits from Base Decorator TODO toevoegen aan docs
     *            ✓  b) calls the method of its parent in the implementation of the method of Component    
     *         Client
     *            ✓  a) has created an object of the type ConcreteComponent
     *            ✓  b) has created an object of the type ConcreteDecorator, to which it passes the ConcreteComponent
     *            ✓  c) has called the method of ConcreteDecorator
     */

file interface IComponent
{
    void Behaviour();
}

file class ConcreteComponent : IComponent
{
    public void Behaviour() { }
}

abstract file class BaseDecorator : IComponent
{
    private IComponent _component;

    public BaseDecorator(IComponent component)
    {
        _component = component;
    }

    public virtual void Behaviour()
    {
        _component.Behaviour();
    }
}

file class ConcreteDecorator : BaseDecorator
{
    public ConcreteDecorator(IComponent component) : base(component) { }

    public override void Behaviour()
    {
        ExtraBehaviour();
        base.Behaviour();
    }

    private void ExtraBehaviour() { }
}

file class Client
{
    private IComponent _component = new ConcreteDecorator(new ConcreteComponent());

    public void Execute()
    {
        _component.Behaviour();
    }
}
