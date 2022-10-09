using System;
using System.Collections.Generic;
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
    /// Interaction logic for SingleHexPicker.xaml
    /// </summary>
    public partial class SingleHexPicker : UserControl
    {

        private HexPatternPickerVM _vm;
        public SingleHexPicker()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is HexPatternPickerVM picker)
            {
                _vm = picker;
                BuildPieces();
                _vm.HexPieceBrushes.CollectionChanged += (s, ev) => BuildPieces();
            }
        }

        private void BuildPieces()
        {
            foreach (Polygon poly in HexCanvas.Children)
            {
                poly.MouseEnter -= OnHexPieceMouseEnter;
                poly.MouseLeave -= OnHexPieceMouseLeave;
                poly.MouseDown -= OnHexPieceClick;
            }
            HexCanvas.Children.Clear();

            var polygons = HexTile.CreateHexagonPolygons(_vm.HexPieceBrushes.Count);
            foreach (var poly in polygons)
            {
                poly.Cursor = Cursors.Hand;
                poly.Tag = polygons.IndexOf(poly);
                poly.Stroke = Brushes.Black;
                poly.StrokeThickness = 1;
                poly.MouseEnter += OnHexPieceMouseEnter;
                poly.MouseLeave += OnHexPieceMouseLeave;
                poly.MouseDown += OnHexPieceClick;

                var fillBind = new Binding(nameof(HexPatternPickerVM.HexPieceBrushes) + $"[{poly.Tag}]");
                BindingOperations.SetBinding(poly, Shape.FillProperty, fillBind);
                HexCanvas.Children.Add(poly);
            }
        }

        private void OnHexPieceClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Polygon poly)
            {
                _vm.SetBrush((int)poly.Tag);
            }
        }

        private void OnHexPieceMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Polygon poly)
            {
                poly.Stroke = Brushes.Black;
                poly.Cursor = Cursors.Hand;
            }
        }

        private void OnHexPieceMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Polygon poly)
            {
                poly.Stroke = Brushes.Yellow;
                poly.Cursor = Cursors.Cross;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.Rotation += 60;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //minus
            if (_vm.HexPieceBrushes.Count > 1)
                _vm.HexPieceBrushes.Remove(_vm.HexPieceBrushes.Last());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //plus
            _vm.HexPieceBrushes.Add(new SolidColorBrush(_vm.Brush.Color));
        }
    }
}
