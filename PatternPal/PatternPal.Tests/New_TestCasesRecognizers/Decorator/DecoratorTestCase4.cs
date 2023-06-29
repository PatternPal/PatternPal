namespace PatternPal.Tests.New_TestCasesRecognizers.Decorator;

/* Pattern:              Decorator
     * Original code source: None
     * 
     * 
     * Requirements to fullfill the pattern:
     *         Component interface
     *            ✓  a) is an interface	/ abstract class
     *            ✓  b) has declared a method
     *                     i) if the class is an abstract class instead of an interface the method has to be an abstract method
     *         Concrete Component
     *            ✓  a) is an implementation of Component
     *            ✓  b) does not have a field of type Component
     *               c) if Component is an abstract class, it overrides the method of Component
     *         Base Decorator
     *            ✓  a) is an implementation of Component
     *            ✓  b) is an abstract class
     *            ✓  c) has a field of type Component
     *            ✓  d) the field is private
     *            ✓  e) has a non-private constructor with a parameter of type Component, which it passes to its field
     *            ✓  f) calls the method of its field in the implementation of the method of Component
     *                     i) if Component is an abstract class, it overrides the method of Component
     *         Concrete Decorator
     *            ✓  a) inherits from Base Decorator
     *            ✓  b) calls the method of its parent in the implementation of the method of Component
     *            ✓  c) has a function providing extra behaviour which it calls in the implementation of the method of Component
     *            ✓  d) the function providing extra behaviour does not use the method of Component
     *         Client
     *            ✓  a) has created an object of the type ConcreteComponent
     *            ✓  b) has created an object of the type ConcreteDecorator, to which it passes the ConcreteComponent
     *            ✓  c) has called the method of ConcreteDecorator
     */

abstract file class Component
{
    public void Behaviour() { }
}

file class ConcreteComponent : Component
{
    public new void Behaviour() { }
}

abstract file class BaseDecorator : Component
{
    private Component _component;

    public BaseDecorator(Component component)
    {
        _component = component;
    }

    new void Behaviour()
    {
        _component.Behaviour();
    }
}

file class ConcreteDecorator : BaseDecorator
{
    public ConcreteDecorator(Component component) : base(component) { }

    new void Behaviour()
    {
        ExtraBehaviour();
        base.Behaviour();
    }

    private void ExtraBehaviour() { }
}

file class Client
{
    private Component _component = new ConcreteDecorator(new ConcreteComponent());

    public void Execute()
    {
        _component.Behaviour();
    }
}
