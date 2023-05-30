using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternPal.Tests.New_TestCasesRecognizers.Adapter
{
    /* Pattern:              Adapter
     * Original code source: None
     *
     *
     * Requirements to fullfill the pattern:
     *         Service
     *            ✓  a) does not inherit from the Client Interface
     *            ✓  b) is used by the Adapter class
     *         Client
     *            ✓  a) has created an object of the type Adapter
     *            ✓  b) has used a method of the Service via the Adapter
     *            ✓  c) has not used a method of the Service without the adapter
     *         Client interface
     *            ✓  a) is an interface/abstract class
     *            ✓  b) is inherited/implemented by an Adapter
     *            ✓  c) contains a method
     *            ✓        1) if it is an abstract class the method should be abstract or virtual
     *         Adapter
     *            ✓  a) inherits/implements the Client Interface
     *            ✓  b) creates an Service object
     *            ✓  c) contains a private field in which the Service is stored
     *            ✓  d) uses the Service class
     *            ✓  e) does not return an instance of the Service
     *            ✓  f) every method uses the Service class
     */

    //Client interface
    public abstract class RectangleInterface
    {
        private int X, Y;
        private float Width, Height;

        public virtual int getX () { return X; }
        public virtual int getY () { return Y; }
        public virtual float getWidth () { return Width; }
        public virtual float getHeight () { return Height; }


        protected RectangleInterface(int x, int y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public virtual void Move(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }

        public virtual void Resize(int dWidth, int dHeight)
        {
            Width += dWidth;
            Height += dHeight;
        }
    }

    //Class not in the pattern
    public class Rectangle : RectangleInterface
    {
        public Rectangle(int x, int y, int width, int height) : base(x, y, width, height){}
    }

    //Service
    class SquareThirdParty
    {
        public int x, y, size;

        public SquareThirdParty(int X, int Y, int Size)
        {
            x = X + 10;
            y = Y + 10;
            size = Size;
        }

        public void MoveRectangleTo(int newX, int newY)
        {
            x = newX;
            y = newY;
        }

        public void MultiplySize(float sizeFactor)
        {
            size = (int)(size * sizeFactor);
        }
    }

    //Adapter
    class Adapter : RectangleInterface
    {
        private SquareThirdParty _service;

        new int getX() { return _service.x - 10;}
        new int getY() { return _service.y - 10;}
        new float getWidth() { return _service.size; }
        new float getHeight() { return _service.size; }

        public Adapter(int X, int Y, float Width, float Height) : base(X, Y, Width, Width)
        {
            _service = new SquareThirdParty(X, Y, (int)Width);
        }
        new void Move(int dx, int dy)
        {
            _service.MoveRectangleTo(getX() + dx, getY() + dy);
        }

        new void Resize(int dWidth, int dHeight)
        {
            _service.MultiplySize((getWidth() + dWidth) / getWidth());
        }
    }

    //Client
    class Client
    {
        void CreateRectangleAndSquare()
        {
            RectangleInterface rectangle = new Rectangle(50, 50, 100, 20);
            Adapter thirdPartySquare = new(100, 100, 25, 100);

            rectangle.Move(10, 0);

            thirdPartySquare.Resize(10, 0);
        }
    }
}
