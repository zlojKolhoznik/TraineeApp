using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TraineeApp.Adornrers;
using TraineeApp.Extensions;

namespace TraineeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private List<Figures.Figure> _figures;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0,  0,  0,  0,  200);
            _timer.Start();
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            cnMain.Refresh();
        }

        private void cnMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Canvas canvas)
            {
                return;
            }
            var adornedLayer = AdornerLayer.GetAdornerLayer(canvas);
            var fd = new FiguresDrawer(canvas);
            adornedLayer.Add(fd);
            _figures = fd.Figures;
        }
    }
}