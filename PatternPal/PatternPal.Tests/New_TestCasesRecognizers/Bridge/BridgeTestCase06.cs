namespace PatternPal.Tests.New_TestCasesRecognizers.Bridge;

//This test misses all the viable criteria for a bridge implementation.
/* Pattern:              Bridge
 * Original code source: none
 *
 * Requirements to fulfill the pattern:
 *         Implementation interface or abstract class:
 *               a) is an interface or abstract class
 *               b) has at least one (if possible: abstract) method
 *         Abstraction class:
 *               a) has a private/protected field or property with the type of the Implementation interface or abstract class
 *            ✓  b) has a method
 *               c) has a method that calls a method in the Implementation interface or abstract class
 *         Concrete Implementation
 *            ✓  a) is an implementation of the Implementation interface or inherits from the 'Implementation' abstract class
 *               b) if Implementation is an abstract class it should override it's abstract methods
 *         Refined Abstraction:
 *            ✓  a) inherits from the Abstraction class
 *            ✓  b) has an method
 *         Client class: 
 *            ✓  a) uses a method in the Abstraction class
 *            ✓  b) creates a Concrete Implementation instance
 *            ✓  c) uses the field or property in Abstraction
 */

// Implementation class
file class Color
{
    internal void Paint()
    {
        Console.WriteLine("Just paint with some coler");
    }

    internal void Draw()
    {
        Console.WriteLine("Just draw with some coler");
    }
}

// Abstraction class
file class Shape
{
    internal Color _color;

    public Shape(Color color)
    {
        _color = color;
    }

    internal void PaintColor()
    {
        Console.WriteLine("Could have painted with a color");
    }

}

// Concrete implementation
file class Red : Color
{
    internal new void Paint()
    {
        Console.WriteLine("Paint with Red");
    }
    internal new void Draw()
    {

        Console.WriteLine("Draw with Red");
    }
}

// Refined Abstraction
file class Circle : Shape
{
    internal Circle (Color color) : base(color)
    { }

    internal void DrawColor()
    {
        this._color.Draw();
    }
}

// Client class
file class Client
{
    internal Client()
    {
        this.GoPaint();
        Circle circle = new Circle(new Red());
        circle.DrawColor();
    }

    private void GoPaint()
    {
        Shape shape = new Shape(new Red());
        shape.PaintColor();
    }


}
