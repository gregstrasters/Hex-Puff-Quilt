using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quilt
{
    /// <summary>
    /// Interaction logic for HexTile.xaml
    /// </summary>
    public partial class HexTile : UserControl
    {
        public static readonly DependencyProperty HexProperty = DependencyProperty.Register("Hex", typeof(HexVM), 
            typeof(HexTile), new FrameworkPropertyMetadata(null, OnColorPatternsChanged));

        public HexVM Hex
        {
            get => (HexVM)GetValue(HexProperty);
            set => SetValue(HexProperty, value);
        }

        private bool _isHovering;
        private Polygon _highlight;
        private Canvas _polygons;
        private Canvas _borders;

        public HexTile()
        {
            _highlight = CreateHighlight();
            _highlight.MouseLeftButtonDown += (s, e) => OnClick();
            _highlight.MouseRightButtonDown += (s, e) => OnRightClick();
            _highlight.MouseEnter += (s, e) => OnMouseEnter();
            _highlight.MouseLeave += (s, e) => OnMouseLeave();

            var hexagon = new Canvas();
            _polygons = new Canvas { Height = 100, Width = 86 };
            _borders = new Canvas();
            hexagon.Children.Add(_polygons);
            hexagon.Children.Add(_borders);
            hexagon.Children.Add(_highlight);

            InitializeComponent();
            HexCanvas.Children.Add(hexagon);
        }

        private static void OnColorPatternsChanged(DependencyObject hexTile, DependencyPropertyChangedEventArgs e)
        {
            if (hexTile is HexTile tile)
            {
                tile.DataContext = e.NewValue;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(sender is HexTile tile && e.NewValue is HexVM hex)
            {
                tile.CreateColorPatterns();

                var rotate = new RotateTransform(hex.Rotation, 43, 50);
                _polygons.RenderTransform = rotate;
                var binding = new Binding(nameof(HexVM.Rotation));
                BindingOperations.SetBinding(rotate, RotateTransform.AngleProperty, binding);

                hex.PropertyChanged += OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object s, PropertyChangedEventArgs e)
        {
            var properties = new string[] { nameof(HexVM.ColorPatterns), nameof(HexVM.IsSelected)};
            if(s is HexVM && properties.Contains(e.PropertyName))
            {
                CreateColorPatterns();
                if (!Hex.IsSelected && !_isHovering)
                    _highlight.Fill = Brushes.Transparent;
            }
        }

        public void CreateColorPatterns()
        {
            _polygons.Children.Clear();
            int totalCount = Hex.ColorPatterns.Sum(c => c.Count);

            var polygons = CreateHexagonPolygons(totalCount);

            var index = 0;
            foreach (var pattern in Hex.ColorPatterns)
            {
                for (int i = 0; i < pattern.Count; i++)
                {
                    polygons[index].Fill = pattern.Color;
                    _polygons.Children.Add(polygons[index]);
                    index++;
                }
            }

            CreateBorders();
        }

        public static List<Polygon> CreateHexagonPolygons(int totalCount)
        {
            var polygons = new List<Polygon>();

            for (int i = 0; i < totalCount / 2; i++)
            {
                int x1 = 86 * i / totalCount;
                int x2 = 86 * (i + 1) / totalCount + 1;
                int y1 = 75 + 50 * i / totalCount;
                int y2 = 75 + 50 * (i + 1) / totalCount;
                int y3 = 25 - 50 * (i + 1) / totalCount;
                int y4 = 25 - 50 * i / totalCount;

                var polygon = new Polygon();
                polygon.Points.Add(new Point(x1, y1));
                polygon.Points.Add(new Point(x2, y2));
                polygon.Points.Add(new Point(x2, y3));
                polygon.Points.Add(new Point(x1, y4));

                polygons.Add(polygon);
            }

            int nextId = totalCount / 2;
            if (totalCount % 2 != 0)
            {
                int x1 = 43 - 43 / totalCount;
                int x2 = 43;
                int x3 = 43 + 43 / totalCount;
                int y1 = 100 - 25 / totalCount;
                int y2 = 100;
                int y3 = 25 / totalCount;
                int y4 = 0;

                var polygon = new Polygon();
                polygon.Points.Add(new Point(x1, y1));
                polygon.Points.Add(new Point(x2, y2));
                polygon.Points.Add(new Point(x3, y1));
                polygon.Points.Add(new Point(x3, y3));
                polygon.Points.Add(new Point(x2, y4));
                polygon.Points.Add(new Point(x1, y3));

                polygons.Add(polygon);
                nextId++;
            }

            for (int i = nextId; i < totalCount; i++)
            {
                int x1 = 86 * i / totalCount;
                int x2 = 86 * (i + 1) / totalCount;
                int y1 = 75 + 50 * (totalCount - i) / totalCount;
                int y2 = 75 + 50 * (totalCount - (i + 1)) / totalCount;
                int y3 = 25 - 50 * (totalCount - (i + 1)) / totalCount;
                int y4 = 25 - 50 * (totalCount - i) / totalCount;

                var polygon = new Polygon();
                polygon.Points.Add(new Point(x1, y1));
                polygon.Points.Add(new Point(x2, y2));
                polygon.Points.Add(new Point(x2, y3));
                polygon.Points.Add(new Point(x1, y4));

                polygons.Add(polygon);
            }

            return polygons;
        }

        private void OnMouseEnter()
        {
            _isHovering = true;
            _highlight.Fill = new SolidColorBrush(Color.FromArgb(150, 150, 150, 150));
        }

        private void OnMouseLeave()
        {
            _isHovering = false;
            if(!Hex.IsSelected)
                _highlight.Fill = Brushes.Transparent;
        }

        private void OnClick()
        {
            Hex.IsSelected = !Hex.IsSelected;
        }

        private void OnRightClick()
        {
            Hex.Rotation += 60;
        }

        private void CreateBorders()
        {
            int strokeThickness = 5;
            var brush = Brushes.Black;
            _borders.Children.Clear();
            if (Hex.Border.BottomLeft)
                _borders.Children.Add(new Line { X1 = 0, X2 = 43, Y1 = 75, Y2 = 100, Stroke = brush, StrokeThickness = strokeThickness });
            if (Hex.Border.Left)
                _borders.Children.Add(new Line { X1 = 0, X2 = 0, Y1 = 25, Y2 = 75, Stroke = brush, StrokeThickness = strokeThickness });
            if (Hex.Border.TopLeft)
                _borders.Children.Add(new Line { X1 = 0, X2 = 43, Y1 = 25, Y2 = 0, Stroke = brush, StrokeThickness = strokeThickness });
            if (Hex.Border.BottomRight)
                _borders.Children.Add(new Line { X1 = 43, X2 = 86, Y1 = 100, Y2 = 75, Stroke = brush, StrokeThickness = strokeThickness });
            if (Hex.Border.Right)
                _borders.Children.Add(new Line { X1 = 86, X2 = 86, Y1 = 75, Y2 = 25, Stroke = brush, StrokeThickness = strokeThickness });
            if (Hex.Border.TopRight)
                _borders.Children.Add(new Line { X1 = 86, X2 = 43, Y1 = 25, Y2 = 0, Stroke = brush, StrokeThickness = strokeThickness });
        }

        private Polygon CreateHighlight()
        {
            var polygon = new Polygon()
            {
                Points =
                {
                    new Point(0, 25),
                    new Point(0, 75),
                    new Point(43, 100),
                    new Point(86, 75),
                    new Point(86, 25),
                    new Point(43, 0),
                },
                Fill = Brushes.Transparent
            };

            return polygon;
        }
    }
}
