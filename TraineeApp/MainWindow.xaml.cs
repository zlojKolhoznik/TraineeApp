using Figures;
using Microsoft.Win32;
using MyRandomizer;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using TraineeApp.Adornrers;
using TraineeApp.IO;
using Figure = Figures.Figure;

namespace TraineeApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private readonly List<Figure> _figures;
    private readonly FiguresDrawer _figuresDrawer;

    private TreeViewItem? _selectedNode;
    private List<ComboBoxItem> _languages;
    private string _currentLanguage;

    public MainWindow()
    {
        InitializeComponent();
        _timer = new DispatcherTimer();
        InitializeTimer();

        _figuresDrawer = new FiguresDrawer(cnMain);
        _figures = _figuresDrawer.Figures;

        InitializeLanguageMenu();
    }

    private void InitializeLanguageMenu()
    {
        var eng = CreateLanguageOption("English");
        var ukr = CreateLanguageOption("Ukrainian");
        _languages = [eng, ukr];
        _currentLanguage = "English";
        cbLanguages.ItemsSource = _languages;
        cbLanguages.SelectedItem = _currentLanguage;
    }

    private ComboBoxItem CreateLanguageOption(string language)
    {
        var item = new ComboBoxItem { Tag = language };
        item.SetResourceReference(ContentProperty, language);
        return item;
    }

    private void InitializeTimer()
    {
        _timer.Tick += Timer_Tick;
        _timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
        _timer.Start();
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
        var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
        adornerLayer.Add(_figuresDrawer);
    }

    private void Triangle_Click(object sender, RoutedEventArgs e)
    {
        var figure = CreateFigure<Triangle>();
        _figures.Add(figure);
        TreeViewItem node = CreateNodeFromFigure(figure);
        tvMain.Items.Add(node);
    }

    private void Circle_Click(object sender, RoutedEventArgs e)
    {
        var figure = CreateFigure<Circle>();
        _figures.Add(figure);
        TreeViewItem node = CreateNodeFromFigure(figure);
        tvMain.Items.Add(node);
    }

    private void Rectangle_Click(object sender, RoutedEventArgs e)
    {
        var figure = CreateFigure<Rectangle>();
        _figures.Add(figure);
        TreeViewItem node = CreateNodeFromFigure(figure);
        tvMain.Items.Add(node);
    }

    private void Restart_Click(object sender, RoutedEventArgs e)
    {
        ClearField();
    }

    private void ClearField()
    {
        _figures.Clear();
        tvMain.Items.Clear();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var sfd = new SaveFileDialog
        {
            Filter = "Binary files (*.bin)|*.bin|JSON files (*.json)|*.json|XML files (*.xml)|*.xml",
            FilterIndex = 1
        };
        if (sfd.ShowDialog() == true)
        {
            switch (sfd.FilterIndex)
            {
                case 1:
                    FigureSerializer.SaveToBinary(sfd.FileName, _figures);
                    break;
                case 2:
                    FigureSerializer.SaveToJson(sfd.FileName, _figures);
                    break;
                case 3:
                    FigureSerializer.SaveToXml(sfd.FileName, _figures);
                    break;
            }
        }
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var ofd = new OpenFileDialog
        {
            Filter = "Binary files (*.bin)|*.bin|JSON files (*.json)|*.json|XML files (*.xml)|*.xml",
            FilterIndex = 1
        };
        if (ofd.ShowDialog() == true)
        {
            ClearField();
            switch (ofd.FilterIndex)
            {
                case 1:
                    _figures.AddRange(FigureSerializer.ReadFromBinary(ofd.FileName));
                    foreach (var figure in _figures)
                    {
                        TreeViewItem node = CreateNodeFromFigure(figure);
                        tvMain.Items.Add(node);
                    }
                    break;
                case 2:
                    _figures.AddRange(FigureSerializer.ReadFromJson(ofd.FileName));
                    foreach (var figure in _figures)
                    {
                        TreeViewItem node = CreateNodeFromFigure(figure);
                        tvMain.Items.Add(node);
                    }
                    break;
                case 3:
                    _figures.AddRange(FigureSerializer.ReadFromXml(ofd.FileName));
                    foreach (var figure in _figures)
                    {
                        TreeViewItem node = CreateNodeFromFigure(figure);
                        tvMain.Items.Add(node);
                    }
                    break;
            }
        }
    }

    private void TvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is TreeViewItem node && node.Tag is Figure figure)
        {
            _selectedNode = node;
            (btnToggleMoving.IsEnabled, btnRemoveBeep.IsEnabled, btnAddBeep.IsEnabled) = (true, true, true);
            btnToggleMoving.SetResourceReference(ContentProperty, figure.IsMoving ? "Stop" : "Move");
        }
        else
        {
            (btnToggleMoving.IsEnabled, btnRemoveBeep.IsEnabled, btnAddBeep.IsEnabled) = (false, false, false);
            btnToggleMoving.SetResourceReference(ContentProperty, "Stop");
        }
    }

    private void ToggleMoving_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedNode is not TreeViewItem node || node.Tag is not Figure figure)
        {
            return;
        }

        figure.IsMoving = !figure.IsMoving;
        if (sender is not Button btn)
        {
            return;
        }

        btn.SetResourceReference(ContentProperty, figure.IsMoving ? "Stop" : "Move");
    }

    private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox)
        {
            return;
        }

        _currentLanguage = (comboBox.SelectedItem as ComboBoxItem)!.Tag.ToString()!;
        var app = Application.Current;
        var resources = app?.Resources;
        if (resources is null)
        {
            return;
        }

        var language = resources.MergedDictionaries[1];
        language.Source = new Uri($"Styles/{_currentLanguage}.xaml", UriKind.Relative);
    }

    private void AddBeep_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedNode is not TreeViewItem node || node.Tag is not Figure figure)
        {
            return;
        }

        figure.Collided += Figure_Collided;
    }

    private void RemoveBeep_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedNode is not TreeViewItem node || node.Tag is not Figure figure)
        {
            return;
        }

        figure.Collided -= Figure_Collided;
    }

    private Figure CreateFigure<T>() where T : Figure
    {
        var pMax = new Point(cnMain.RenderSize.Width, cnMain.RenderSize.Height);
        var randomizer = new Randomizer();
        var color = randomizer.NextColor();
        var width = randomizer.NextInt(10, 50);
        var height = randomizer.NextInt(10, 50);
        return Figure.Create<T>(pMax, color, width, height);
    }

    private void Figure_Collided(object? sender, CollidedEventArgs e)
    {
        SystemSounds.Beep.Play();
        lblLastCollisionCoords.Content = $"x={e.Coordinates.X}, y={e.Coordinates.Y}";
    }

    private TreeViewItem CreateNodeFromFigure(Figure figure)
    {
        var node = new TreeViewItem
        {
            Tag = figure,
            Foreground = new SolidColorBrush(figure.Color),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        node.SetResourceReference(TreeViewItem.HeaderProperty, figure.GetType().Name);
        node.SetResourceReference(StyleProperty, "TreeViewItemStyle");
        return node;
    }

}