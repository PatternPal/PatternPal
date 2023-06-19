using System;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Console = System.Console;


namespace PatternPal.Tests.New_TestCasesRecognizers.Bridge
{

    //This test is a possible "perfect" bridge implementation.
    /* Pattern:              Bridge
     * Original code source: none
     *
     * Requirements to fulfill the pattern:
     *         Implementation interface or abstract class:
     *            ✓  a) is an interface or abstract class
     *            ✓  b) has at least one (if possible: abstract) method
     *         Abstraction class:
     *            ✓  a) has a private/protected field or property with the type of the Implementation interface or abstract class
     *            ✓  b) has a method
     *            ✓  c) has a method that calls a method in the Implementation interface or abstract class
     *            ✓  d) has either,
     *                  1) the property option as described in a, or has
     *            ✓     2) a constructor with a parameter with the Implementation type and that uses the field as described in a, or has
     *                  3) a method with a parameter with the Implementation type and that uses the field as described in a
     *         Concrete Implementation
     *            ✓  a) is an implementation of the Implementation interface or inherits from the 'Implementation' abstract class
     *            ✓  b) if Implementation is an abstract class it should override it's abstract methods
     *         Refined Abstraction:
     *            ✓  a) inherits from the Abstraction class
     *            ✓  b) has an method
     *         Client class: 
     *            ✓  a) uses a method in the Abstraction class
     *            ✓  b) creates a Concrete Implementation instance
     *            ✓  c) sets the field or property in Abstraction, either through
     *                  1) it is a property and it sets this, or through
     *            ✓     2) a constructor as described in Abstraction d2
     *                  3) a method as described in Abstraction d3
     */

    // Implementation class
    abstract file class Color
    {
         internal abstract void Paint();
         internal abstract void Draw();
    }

    // Abstraction class
    file class Shape
    {
        protected internal Color _color;

        public Shape(Color color)
        {
            _color = color;
        }

        internal void PaintColor()
        {
            _color.Paint();
        }

    }

    // Concrete implementation
    file class Red : Color
    {
        internal override void Paint()
        {
            Console.WriteLine("Paint with Red");
        }
        internal override void Draw()
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
}
