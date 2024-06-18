﻿using Figures;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using TraineeApp.Adornrers;

using Figure = Figures.Figure;

namespace TraineeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly List<Figure> _figures;
        private readonly FiguresDrawer _figuresDrawer;
        private TreeViewItem? _selectedNode;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0,  0,  0,  0,  20);
            _timer.Start();
            _figuresDrawer = new FiguresDrawer(cnMain);
            _figures = _figuresDrawer.Figures;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            AdornerLayer.GetAdornerLayer(cnMain).Update();
        }

        private void CnMain_Loaded(object sender, RoutedEventArgs e)
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
            CreateFigure(typeof(Triangle));
        }

        private void Circle_Click(object sender, RoutedEventArgs e) 
        {
            CreateFigure(typeof(Circle));
        }

        private void Rectangle_Click(object sender, RoutedEventArgs e) 
        {
            CreateFigure(typeof(Rectangle));
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            _figures.Clear();
            tvMain.Items.Clear();
        }

        private void TvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem node && node.Tag is Figure)
            {
                _selectedNode = node;
                btnDelete.IsEnabled = true;
            }
            else
            {
                btnDelete.IsEnabled = false;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedNode is not TreeViewItem node || node.Tag is not Figure figure)
            {
                return;
            }

            _figures.Remove(figure);
            tvMain.Items.Remove(node);
        }

        private void CreateFigure(Type type)
        {
            if (!type.IsAssignableTo(typeof(Figure)))
            {
                throw new ArgumentException("Type must be a subclass of Figure");
            }

            var pMax = new Point(cnMain.RenderSize.Width, cnMain.RenderSize.Height);
            var color = GetRandomColor();
            if (Activator.CreateInstance(type, pMax, 50, 50, color) is not Figure figure)
            {
                throw new InvalidOperationException("Failed to create figure");
            }

            _figures.Add(figure);
            CreateNodeFromFigure(figure);
        }

        private void CreateNodeFromFigure(Figure figure)
        {
            var node = new TreeViewItem
            {
                Tag = figure,
                Header = $"{figure.GetType().Name} #{_figures.Count(f => f.GetType() == figure.GetType())}",
                Foreground = new SolidColorBrush(figure.Color),
                HorizontalAlignment = HorizontalAlignment.Left
            };
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