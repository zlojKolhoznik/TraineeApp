using System.Windows;
using System.Windows.Media;

namespace Figures
{
    public abstract class Figure
    {
        protected int _sizeX;           // half-width
        protected int _sizeY;           // half-height
        protected Point _coords;
        protected Point _speedVector;
        protected readonly Color _color;

        public Figure(Point pMax, int width, int height, Color color)
        {
            var rnd = new Random();
            _sizeX = width / 2;
            _sizeY = height / 2;
            _coords = new Point(rnd.Next(_sizeX, (int)pMax.X - _sizeX), rnd.Next(_sizeY, (int)pMax.Y - _sizeY));
            _speedVector = new Point(rnd.Next(5, 10), rnd.Next(5, 10));
            _color = color;
        }

        public Color Color => _color;

        public bool IsMoving { get; set; } = true;

        public void Move(Point pMax)
        {
            if (!IsMoving)
            {
                return;
            }

            if (Left < 0)
            {
                _speedVector.X = Math.Abs(_speedVector.X);
            }
            else if (Right > pMax.X)
            {
                _speedVector.X = -Math.Abs(_speedVector.X);
            }

            if (Top < 0)
            {
                _speedVector.Y = Math.Abs(_speedVector.Y);
            }
            else if (Bottom > pMax.Y)
            {
                _speedVector.Y = -Math.Abs(_speedVector.Y);
            }

            _coords.X += _speedVector.X;
            _coords.Y += _speedVector.Y;
        }

        public abstract void Draw(DrawingContext dc);

        public int Left => (int)_coords.X - _sizeX;

        public int Right => (int)_coords.X + _sizeX;

        public int Top => (int)_coords.Y - _sizeY;

        public int Bottom => (int)_coords.Y + _sizeY;
    }

}
