using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace Quilt
{
    public class HexPatternPickerVM : BaseVM
    {
        private string _hexColor = "#";
        private int _rotation;
        private ObservableCollection<SolidColorBrush> _hexPieceBrushes = new ObservableCollection<SolidColorBrush>
        {
            new SolidColorBrush(Colors.Transparent)
        };
        private bool _isSelected = true;
        private int _count = 1;

        public string HexColor
        {
            get => _hexColor;
            set
            {
                Set(ref _hexColor, value);

                if (Regex.IsMatch(value, "^#(?:[0-9a-fA-F]{3}){1,2}$"))
                {
                    Brush.Color = (Color)ColorConverter.ConvertFromString(_hexColor);
                    RaisePropertyChangedEvent(nameof(Brush));
                }
            }
        }

        public SolidColorBrush Brush { get; } = new SolidColorBrush(Colors.Transparent);

        public int Rotation
        {
            get => _rotation;
            set => Set(ref _rotation, value);
        }

        public ObservableCollection<SolidColorBrush> HexPieceBrushes
        {
            get => _hexPieceBrushes;
            set => Set(ref _hexPieceBrushes, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
        public int Count
        {
            get => _count;
            set
            {
                if (value > 0)
                    Set(ref _count, value);
            }
        }

        public List<ColorPattern> GetColorPatterns()
        {
            return new List<ColorPattern>(HexPieceBrushes.Select(b => new ColorPattern(1, b)));
        }

        public void SetBrush(int index)
        {
            HexPieceBrushes[index] = new SolidColorBrush(Brush.Color);
            RaisePropertyChangedEvent(nameof(HexPieceBrushes));
        }
    }
}
