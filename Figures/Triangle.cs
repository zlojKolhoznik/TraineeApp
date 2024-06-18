using System.Windows;
using System.Windows.Media;

namespace Figures
{
    public class Triangle : Figure
    {
        public Triangle(Point pMax, int width, int height, Color color) : base(pMax, width, height, color)
        {
        }

        public override void Draw(DrawingContext dc)
        {
            var a = new Point(_coords.X - _sizeX, _coords.Y + _sizeY);
            var b = new Point(_coords.X + _sizeX, _coords.Y + _sizeY);
            var c = new Point(_coords.X, _coords.Y - _sizeY);
            List<LineSegment> segments = [new LineSegment(b, true), new LineSegment(c, true)];
            var figure = new PathFigure(a, segments, true);
            var geometry = new PathGeometry([figure]);
            var brush = new SolidColorBrush(_color);
            var pen = new Pen(brush, 1);
            dc.DrawGeometry(brush, pen, geometry);
        }
    }
}
