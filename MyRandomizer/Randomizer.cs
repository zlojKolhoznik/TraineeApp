using System.Windows;
using System.Windows.Media;

namespace MyRandomizer;

public class Randomizer
{
    private readonly Random _random;

    public Randomizer()
    {
        _random = new();
    }

    public Point NextPoint(Point pMax) => new(_random.Next((int)pMax.X), _random.Next((int)pMax.Y));

    public Color NextColor() => Color.FromRgb((byte)_random.Next(256), (byte)_random.Next(256), (byte)_random.Next(256));

    public Point NextVector(int minLength, int maxLength)
    {
        var result = new Point(_random.Next(-maxLength, maxLength), _random.Next(-maxLength, maxLength));
        while (Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) > Math.Pow(maxLength, 2) ||
            Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) < Math.Pow(minLength, 2))
        {
            result = new Point(_random.Next(-maxLength, maxLength), _random.Next(-maxLength, maxLength));
        }

        return result;
    }

    public int NextInt(int min, int max) => _random.Next(min, max);
}
