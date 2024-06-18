using Figures;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using TraineeApp.Adornrers;

namespace TraineeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private List<Figures.Figure> _figures;
        private FiguresDrawer _figuresDrawer;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0,  0,  0,  0,  20);
            _timer.Start();
            _figuresDrawer = new FiguresDrawer(cnMain);
            _figures = _figuresDrawer.Figures;
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            AdornerLayer.GetAdornerLayer(cnMain).Update();
        }

        private void cnMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Canvas canvas)
            {
                return;
            }
            var adornedLayer = AdornerLayer.GetAdornerLayer(canvas);
            adornedLayer.Add(_figuresDrawer);
        }

        private void Triangle_Click(object sender, RoutedEventArgs e)
        {
            var pMax = new Point(cnMain.RenderSize.Width, cnMain.RenderSize.Height);
            var rnd = new Random();
            var rgb = new byte[3];
            rnd.NextBytes(rgb);
            Color color = Color.FromRgb(rgb[0], rgb[1], rgb[2]);
            var triangle = new Triangle(pMax, 50, 50, color);
            var node = new TreeViewItem();
            _figures.Add(triangle);
            node.Tag = triangle;
            node.Header = $"Triangle #{_figures.Count(f => f is Triangle)}";
            node.Foreground = new SolidColorBrush(color);
            node.HorizontalAlignment = HorizontalAlignment.Center;
            tvMain.Items.Add(node);
        }

        private void Circle_Click(object sender, RoutedEventArgs e) 
        {
            var pMax = new Point(cnMain.RenderSize.Width, cnMain.RenderSize.Height);
            var rnd = new Random();
            var rgb = new byte[3];
            rnd.NextBytes(rgb);
            Color color = Color.FromRgb(rgb[0], rgb[1], rgb[2]);
            var circle = new Circle(pMax, 50, 50, color);
            var node = new TreeViewItem();
            _figures.Add(circle);
            node.Tag = circle;
            node.Header = $"Circle #{_figures.Count(f => f is Circle)}";
            node.Foreground = new SolidColorBrush(color);
            node.HorizontalAlignment = HorizontalAlignment.Center;
            tvMain.Items.Add(node);
        }

        private void Rectangle_Click(object sender, RoutedEventArgs e) 
        {
            var pMax = new Point(cnMain.RenderSize.Width, cnMain.RenderSize.Height);
            var rnd = new Random();
            var rgb = new byte[3];
            rnd.NextBytes(rgb);
            Color color = Color.FromRgb(rgb[0], rgb[1], rgb[2]);
            var rectangle = new Figures.Rectangle(pMax, 50, 50, color);
            var node = new TreeViewItem();
            _figures.Add(rectangle);
            node.Tag = rectangle;
            node.Header = $"Rectangle #{_figures.Count(f => f is Figures.Rectangle)}";
            node.Foreground = new SolidColorBrush(color);
            node.HorizontalAlignment = HorizontalAlignment.Center;
            tvMain.Items.Add(node);
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            _figures.Clear();
            tvMain.Items.Clear();
        }

        private void CreateFigure(Type type)
        {
            if (!type.IsAssignableTo(typeof(Figures.Figure)))
            {
                throw new ArgumentException("Type must be a subclass of Figure");
            }

            var pMax = new Point(cnMain.RenderSize.Width, cnMain.RenderSize.Height);
            var color = GetRandomColor();
            if (Activator.CreateInstance(type, pMax, 50, 50, color) is not Figures.Figure figure)
            {
                throw new InvalidOperationException("Failed to create figure");
            }

            CreateNodeFromFigure(figure, color);
        }

        private void CreateNodeFromFigure(Figures.Figure figure, Color color)
        {
            var node = new TreeViewItem();
            _figures.Add(figure);
            node.Tag = figure;
            node.Header = $"{figure.GetType().Name} #{_figures.Count(f => f.GetType() == figure.GetType())}";
            node.Foreground = new SolidColorBrush(color);
            node.HorizontalAlignment = HorizontalAlignment.Center;
            tvMain.Items.Add(node);
        }

        private static Color GetRandomColor()
        {
            var rnd = new Random();
            var rgb = new byte[3];
            rnd.NextBytes(rgb);
            return Color.FromRgb(rgb[0], rgb[1], rgb[2]);
        }
    }
}