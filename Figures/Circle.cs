using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Figures;

[Serializable]
public class Circle : Figure
{
    public Circle(Point pMax, int width, int height, Color color) : base(pMax, width, height, color)
    {
    }

    protected Circle(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public Circle() : base()
    {
        
    }

    public override FigureType FigureType => FigureType.Circle;


    public override void Draw(DrawingContext dc)
    {
        var brush = new SolidColorBrush(Color);
        var pen = new Pen(brush, 1);
        dc.DrawEllipse(brush, pen, Coordinates, SizeX, SizeY);
    }
}
