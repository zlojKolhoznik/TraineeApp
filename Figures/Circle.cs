using System.Windows;
using System.Windows.Media;

namespace Figures
{
    public class Circle : Figure
    {
        public Circle(Point pMax, int width, int height, Color color) : base(pMax, width, height, color)
        {
        }

        public override void Draw(DrawingContext dc)
        {
            var brush = new SolidColorBrush(_color);
            var pen = new Pen(brush, 1);
            dc.DrawEllipse(brush, pen, _coords, _sizeX, _sizeY);
        }
    }
}
