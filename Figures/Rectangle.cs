using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Figures;

[Serializable]
public class Rectangle : Figure
{
    public Rectangle(Point pMax, int width, int height, Color color) : base(pMax, width, height, color)
    {
    }

    protected Rectangle(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public Rectangle() : base()
    {
        
    }

    public override FigureType FigureType => FigureType.Rectangle;

    public override void Draw(DrawingContext dc)
    {
        var pivot = new Point(Coordinates.X - SizeX, Coordinates.Y - SizeY);
        var brush = new SolidColorBrush(Color);
        var pen = new Pen(brush, 1);
        var rectangle = new Rect(pivot, new Size(SizeX * 2, SizeY * 2));
        dc.DrawRectangle(brush, pen, rectangle);
    }
}
