using Figures;
using Microsoft.Win32;
using MyRandomizer;
using System.IO;
using System.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using TraineeApp.Adornrers;

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
                    SaveBinary(sfd.FileName);
                    break;
                case 2:
                    SaveJson(sfd.FileName);
                    break;
                case 3:
                    SaveXml(sfd.FileName);
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
            switch (ofd.FilterIndex)
            {
                case 1:
                    OpenBinary(ofd.FileName);
                    break;
                case 2:
                    OpenJson(ofd.FileName);
                    break;
                case 3:
                    OpenXml(ofd.FileName);
                    break;
            }
        }
    }

    private void TvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is TreeViewItem node && node.Tag is Figure figure)
        {
            _selectedNode = node;
            btnToggleMoving.IsEnabled = true;
            btnRemoveBeep.IsEnabled = true;
            btnAddBeep.IsEnabled = true;
            btnToggleMoving.SetResourceReference(ContentProperty, figure.IsMoving ? "Stop" : "Move");
        }
        else
        {
            btnToggleMoving.IsEnabled = false;
            btnRemoveBeep.IsEnabled = false;
            btnAddBeep.IsEnabled = false;
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

    private void CreateFigure(Type type)
    {
        if (!type.IsAssignableTo(typeof(Figure)))
        {
            throw new ArgumentException("Type must be a subclass of Figure");
        }

        var pMax = new Point(cnMain.RenderSize.Width, cnMain.RenderSize.Height);
        var randomizer = new Randomizer();
        var color = randomizer.NextColor();
        if (Activator.CreateInstance(type, pMax, 50, 50, color) is not Figure figure)
        {
            throw new InvalidOperationException("Failed to create figure");
        }

        _figures.Add(figure);
        CreateNodeFromFigure(figure);
    }

    private void Figure_Collided(object? sender, CollidedEventArgs e)
    {
        SystemSounds.Beep.Play();
        lblLastCollisionCoords.Content = $"x={e.Coordinates.X}, y={e.Coordinates.Y}";
    }

    private void CreateNodeFromFigure(Figure figure)
    {
        var node = new TreeViewItem
        {
            Tag = figure,
            Foreground = new SolidColorBrush(figure.Color),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        node.SetResourceReference(TreeViewItem.HeaderProperty, figure.GetType().Name);
        node.SetResourceReference(StyleProperty, "TreeViewItemStyle");
        tvMain.Items.Add(node);
    }

#pragma warning disable SYSLIB0011 // Type or member is obsolete

    private void SaveBinary(string path)
    {
        using var fs = new FileStream(path, FileMode.Create);
        var formatter = new BinaryFormatter();
        formatter.Serialize(fs, _figures);
    }

#pragma warning restore SYSLIB0011 // Type or member is obsolete


    private void SaveJson(string path)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
        var json = JsonSerializer.Serialize(_figures, options);
        File.WriteAllText(path, json);
    }

    private void SaveXml(string path)
    {
        var serializer = new XmlSerializer(typeof(List<Figure>));
        using var fs = new FileStream(path, FileMode.Create);
        serializer.Serialize(fs, _figures);
    }

#pragma warning disable SYSLIB0011 // Type or member is obsolete

    private void OpenBinary(string path)
    {
        using var fs = new FileStream(path, FileMode.Open);
        var formatter = new BinaryFormatter();
        _figures.Clear();
        _figures.AddRange((List<Figure>)formatter.Deserialize(fs));
        tvMain.Items.Clear();
        foreach (var figure in _figures)
        {
            CreateNodeFromFigure(figure);
        }
    }

#pragma warning restore SYSLIB0011 // Type or member is obsolete

    private void OpenJson(string path)
    {
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
        _figures.Clear();
        _figures.AddRange(JsonSerializer.Deserialize<List<Figure>>(json, options));
        tvMain.Items.Clear();
        foreach (var figure in _figures)
        {
            CreateNodeFromFigure(figure);
        }
    }

    private void OpenXml(string path)
    {
        var serializer = new XmlSerializer(typeof(List<Figure>));
        using var fs = new FileStream(path, FileMode.Open);
        _figures.Clear();
        _figures.AddRange((List<Figure>)serializer.Deserialize(fs));
        tvMain.Items.Clear();
        foreach (var figure in _figures)
        {
            CreateNodeFromFigure(figure);
        }
    }

}