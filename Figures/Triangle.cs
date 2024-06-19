using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Figures;

[Serializable]
public class Triangle : Figure
{
    public Triangle(Point pMax, int width, int height, Color color) : base(pMax, width, height, color)
    {
    }

    protected Triangle(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public Triangle() : base()
    {
        
    }

    public override FigureType FigureType => FigureType.Triangle;


    public override void Draw(DrawingContext dc)
    {
        var a = new Point(Coordinates.X - SizeX, Coordinates.Y + SizeY);
        var b = new Point(Coordinates.X + SizeX, Coordinates.Y + SizeY);
        var c = new Point(Coordinates.X, Coordinates.Y - SizeY);
        List<LineSegment> segments = [new LineSegment(b, true), new LineSegment(c, true)];
        var figure = new PathFigure(a, segments, true);
        var geometry = new PathGeometry([figure]);
        var brush = new SolidColorBrush(Color);
        var pen = new Pen(brush, 1);
        dc.DrawGeometry(brush, pen, geometry);
    }
}
