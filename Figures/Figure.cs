using Figures.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Figures;

[Serializable,
    XmlInclude(typeof(Rectangle)), 
    XmlInclude(typeof(Circle)), 
    XmlInclude(typeof(Triangle)),
    JsonConverter(typeof(FigureJsonConverter))]
public abstract class Figure : ISerializable
{
    public Figure(Point pMax, int width, int height, Color color)
    {
        var rnd = new Random();
        SizeX = width / 2;
        SizeY = height / 2;
        Coordinates = new Point(rnd.Next(SizeX, (int)pMax.X - SizeX), rnd.Next(SizeY, (int)pMax.Y - SizeY));
        SpeedVector = new Point(rnd.Next(5, 10), rnd.Next(5, 10));
        Color = color;
    }

    public Figure() : this(new Point(0, 0), 0, 0, Colors.Black)
    {
        
    }

    protected Figure(SerializationInfo info, StreamingContext context)
    {
        SizeX = info.GetInt32("SizeX");
        SizeY = info.GetInt32("SizeY");
        Coordinates = (Point)info.GetValue("Coords", typeof(Point));
        SpeedVector = (Point)info.GetValue("SpeedVector", typeof(Point));
        IsMoving = info.GetBoolean("IsMoving");
        Color = (Color)ColorConverter.ConvertFromString(info.GetString("Color"));
    }
    public abstract FigureType FigureType { get; }

    public int SizeX { get; set; }              // half-width
    public int SizeY { get; set; }              // half-height
    public Point Coordinates { get; set; }
    public Point SpeedVector { get; set; }
    public Color Color { get; set; }

    public bool IsMoving { get; set; } = true;

    public void Move(Point pMax)
    {
        if (!IsMoving)
        {
            return;
        }

        if (Left < 0)
        {
            SpeedVector = SpeedVector with { X = Math.Abs(SpeedVector.X) };
        }
        else if (Right > pMax.X)
        {
            SpeedVector = SpeedVector with { X = -Math.Abs(SpeedVector.X) };
        }

        if (Top < 0)
        {
            SpeedVector = SpeedVector with { Y = Math.Abs(SpeedVector.Y) };
        }
        else if (Bottom > pMax.Y)
        {
            SpeedVector = SpeedVector with { Y = -Math.Abs(SpeedVector.Y) };
        }

        Coordinates = new Point(Coordinates.X + SpeedVector.X, Coordinates.Y + SpeedVector.Y);
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("SizeX", SizeX);
        info.AddValue("SizeY", SizeY);
        info.AddValue("Coords", Coordinates);
        info.AddValue("SpeedVector", SpeedVector);
        info.AddValue("Color", Color.ToString());
        info.AddValue("IsMoving", IsMoving);
    }

    public abstract void Draw(DrawingContext dc);


    public int Left => (int)Coordinates.X - SizeX;

    public int Right => (int)Coordinates.X + SizeX;

    public int Top => (int)Coordinates.Y - SizeY;

    public int Bottom => (int)Coordinates.Y + SizeY;
}

public enum FigureType
{
    Rectangle,
    Circle,
    Triangle
}
