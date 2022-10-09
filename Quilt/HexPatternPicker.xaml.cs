using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quilt
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class HexPatternPicker : UserControl
    {
        public static readonly RoutedEvent OnRemoveEvent = EventManager.RegisterRoutedEvent(
       "OnRemove", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HexPatternPicker));

        public event RoutedEventHandler OnRemove
        {
            add { AddHandler(OnRemoveEvent, value); }
            remove { RemoveHandler(OnRemoveEvent, value); }
        }

        private HexPatternPickerVM _vm;
        public HexPatternPicker()
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
            if(_vm.HexPieceBrushes.Count > 1)
                _vm.HexPieceBrushes.Remove(_vm.HexPieceBrushes.Last());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //plus
            _vm.HexPieceBrushes.Add(new SolidColorBrush(_vm.Brush.Color));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OnRemoveEvent));
        }
    }
}
