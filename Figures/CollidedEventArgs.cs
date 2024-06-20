using System.Windows;

namespace Figures;

public class CollidedEventArgs : EventArgs
{

    public CollidedEventArgs(Point coordinates) => Coordinates = coordinates;

    public CollidedEventArgs(int x, int y) : this(new Point(x, y))
    {
    }

    public Point Coordinates { get; init; }
}
