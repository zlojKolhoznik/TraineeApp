
using System.Drawing;
using System.Windows.Media;

namespace Figures
{
    public abstract class Figure
    {
        protected int _sizeX;           // half-width
        protected int _sizeY;           // half-height
        protected Point _coords;
        protected Point _speedVector;

        public Figure(Point pMax, int width, int height)
        {
            var rnd = new Random();
            _sizeX = width / 2;
            _sizeY = height / 2;
            _coords = new Point(rnd.Next(pMax.X - _sizeX), rnd.Next(pMax.Y - _sizeY));
            _speedVector = new Point(rnd.Next(5, 10), rnd.Next(5, 10));
        }

        public void Move(Point pMax)
        {
            if (Left < 0 || Right > pMax.X)
            {
                _speedVector.X = -_speedVector.X;
            }

            if (Top < 0 || Bottom > pMax.Y)
            {
                _speedVector.Y = -_speedVector.Y;
            }

            _coords.X += _speedVector.X;
            _coords.Y += _speedVector.Y;
        }

        public abstract void Draw(DrawingContext dc);

        public int Left => _coords.X - _sizeX;

        public int Right => _coords.X + _sizeX;

        public int Top => _coords.Y - _sizeY;

        public int Bottom => _coords.Y + _sizeY;
    }

}
