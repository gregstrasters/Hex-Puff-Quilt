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
    /// Interaction logic for CsvHexPicker.xaml
    /// </summary>
    public partial class CsvHexPicker : UserControl
    {
        private CsvHexPickerVM _vm = new CsvHexPickerVM();
        private Polygon _border;

        public CsvHexPicker()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CsvHexPickerVM picker)
            {
                _vm = picker;
                BuildPieces();
            }
        }

        private void BuildPieces()
        {
            HexCanvas.Children.Clear();

            int totalCount = _vm.ColorPatterns.Sum(c => c.Count);
            var polygons = HexTile.CreateHexagonPolygons(totalCount);
            var index = 0;
            foreach (var pattern in _vm.ColorPatterns)
            {
                for (int i = 0; i < pattern.Count; i++)
                {
                    polygons[index].Fill = pattern.Color;
                    HexCanvas.Children.Add(polygons[index]);
                    index++;
                }
            }

            _border = HexTile.CreateHexagonPolygons(1).First();
            _border.Stroke = Brushes.Black;
            _border.StrokeThickness = 3;
            HexCanvas.MouseDown += OnClick;
            HexCanvas.Children.Add(_border);
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            _vm.IsSelected = !_vm.IsSelected;
            _border.Stroke = _vm.IsSelected ? Brushes.Yellow : Brushes.Black;
        }
    }
}
