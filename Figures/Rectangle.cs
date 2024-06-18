using System.Windows;
using System.Windows.Media;

namespace Figures
{
    public class Rectangle : Figure
    {
        public Rectangle(Point pMax, int width, int height, Color color) : base(pMax, width, height, color)
        {
        }

        public override void Draw(DrawingContext dc)
        {
            var pivot = new Point(_coords.X - _sizeX, _coords.Y - _sizeY);
            var brush = new SolidColorBrush(_color);
            var pen = new Pen(brush, 1);
            var rectangle = new Rect(pivot, new Size(_sizeX * 2, _sizeY * 2));
            dc.DrawRectangle(brush, pen, rectangle);
        }
    }
}
