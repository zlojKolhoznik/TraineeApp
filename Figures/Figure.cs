using Figures.Converters;
using MyRandomizer;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Figures;

[Serializable,
    XmlInclude(typeof(Rectangle)), 
    XmlInclude(typeof(Circle)), 
    XmlInclude(typeof(Triangle)),
    JsonConverter(typeof(FigureJsonConverter))]
public abstract class Figure : ISerializable
{
    public event EventHandler<CollidedEventArgs>? Collided;

    public Figure(Point pMax, int width, int height, Color color)
    {
        var rnd = new Randomizer();
        SizeX = width / 2;
        SizeY = height / 2;
        Coordinates = rnd.NextPoint(pMax);
        SpeedVector = rnd.NextVector(5, 9);
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

    public int Left => (int)Coordinates.X - SizeX;

    public int Right => (int)Coordinates.X + SizeX;

    public int Top => (int)Coordinates.Y - SizeY;

    public int Bottom => (int)Coordinates.Y + SizeY;

    public static Figure Create<T>(Point pMax, Color color, int width, int height) where T : Figure
    {
        return (T?)Activator.CreateInstance(typeof(T), pMax, width, height, color) ?? throw new InvalidOperationException("Failed to create figure");
    }

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
        if (Coordinates.X > pMax.X || Coordinates.X < 0)
        {
            throw new FigureOutOfBoundsException("X out of bounds", (int)Coordinates.X, (int)Coordinates.Y);
        }

        if (Coordinates.Y > pMax.Y || Coordinates.Y < 0)
        {
            throw new FigureOutOfBoundsException("Y out of bounds", (int)Coordinates.X, (int)Coordinates.Y);
        }
    }

    public void ReturnToBounds(Point pMax)
    {
        if (Left < 0)
        {
            Coordinates = new Point(SizeX, Coordinates.Y);
            SpeedVector = SpeedVector with { X = Math.Abs(SpeedVector.X) };
        }
        else if (Right > pMax.X)
        {
            Coordinates = new Point(pMax.X - SizeX, Coordinates.Y);
            SpeedVector = SpeedVector with { X = -Math.Abs(SpeedVector.X) };
        }

        if (Top < 0)
        {
            Coordinates = new Point(Coordinates.X, SizeY);
            SpeedVector = SpeedVector with { Y = Math.Abs(SpeedVector.Y) };
        }
        else if (Bottom > pMax.Y)
        {
            Coordinates = new Point(Coordinates.X, pMax.Y - SizeY);
            SpeedVector = SpeedVector with { Y = -Math.Abs(SpeedVector.Y) };
        }
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

    public void CollisionCheck(IEnumerable<Figure> others)
    {
        Figure? collision = others.Except([this]).Where(f => f.GetType() == GetType())
            .FirstOrDefault(f => Left < f.Right && Right > f.Left && Top < f.Bottom && Bottom > f.Top);
        if (collision is not null)
        {
            int x = (int)(Coordinates.X + collision.Coordinates.X) / 2;
            int y = (int)(Coordinates.Y + collision.Coordinates.Y) / 2;
            OnCollided(new CollidedEventArgs(x, y));
        }
    }

    protected virtual void OnCollided(CollidedEventArgs e)
    {
        var temp = Volatile.Read(ref Collided);
        temp?.Invoke(this, e);
    }

    public abstract void Draw(DrawingContext dc);
}

public enum FigureType
{
    Rectangle,
    Circle,
    Triangle
}
