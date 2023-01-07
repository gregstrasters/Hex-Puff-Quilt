using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Quilt
{
    public enum PaletteTab
    {
        Custom,
        Csv,
        Mass
    }

    public class PaletteTabs : BaseVM
    {
        private string _massHexColor = "#";
        private bool _isFillRegionSelected;
        private bool _isCsvFillRegionSelected;
        private bool _isMassFillRegionSelected;
        private bool _isCustomSelected = true;
        private bool _isCsvSelected;
        private bool _isMassSelected;
        private PaletteTab _tab;
        private double _mass;

        public string MassHexColor
        {
            get => _massHexColor;
            set
            {
                Set(ref _massHexColor, value);

                if (Regex.IsMatch(value, "^#(?:[0-9a-fA-F]{3}){1,2}$"))
                {
                    MassBrush.Color = (Color)ColorConverter.ConvertFromString(_massHexColor);
                    RaisePropertyChangedEvent(nameof(Brush));
                }
            }
        }
        public SolidColorBrush MassBrush { get; } = new SolidColorBrush(Colors.Transparent);
        public List<List<List<ColorPattern>>> CsvColors = CsvUtil.GetPatternsFromCSV();
        public ObservableCollection<CsvHexPickerVM> MassHexes { get; set; } = new ObservableCollection<CsvHexPickerVM>();
        public ObservableCollection<PatternGroupVM> CsvHexes { get; set; } = new ObservableCollection<PatternGroupVM>();
        public int CsvHexCount => CsvHexes.Sum(p => p.Patterns.Count);
        public ObservableCollection<HexPatternPickerVM> HexPatternPickers { get; set; } = new ObservableCollection<HexPatternPickerVM>
        {
             new HexPatternPickerVM()
        };

        public Cursor Cursor => IsFillRegionSelected | IsCsvFillRegionSelected ? Cursors.Cross : Cursors.Hand;

        public Visibility FillRegionSelectedVisibility => IsFillRegionSelected ? Visibility.Visible : Visibility.Hidden;
        public Visibility CsvFillRegionSelectedVisibility => IsCsvFillRegionSelected ? Visibility.Visible : Visibility.Hidden;
        public Visibility MassFillRegionSelectedVisibility => IsMassFillRegionSelected ? Visibility.Visible : Visibility.Hidden;

        public bool IsFillRegionSelected
        {
            get => _isFillRegionSelected;
            set => Set(ref _isFillRegionSelected, value, addtlPropertyChanges: new[] { nameof(Cursor), nameof(FillRegionSelectedVisibility) });
        }
        public bool IsCsvFillRegionSelected
        {
            get => _isCsvFillRegionSelected;
            set => Set(ref _isCsvFillRegionSelected, value, addtlPropertyChanges: new[] { nameof(Cursor), nameof(CsvFillRegionSelectedVisibility) });
        }
        public bool IsMassFillRegionSelected
        {
            get => _isMassFillRegionSelected;
            set => Set(ref _isMassFillRegionSelected, value, addtlPropertyChanges: new[] { nameof(Cursor), nameof(MassFillRegionSelectedVisibility) });
        }
        public PaletteTab Tab
        {
            get => _tab;
            set => Set(ref _tab, value);
        }
        public bool IsCustomSelected
        {
            get => _isCustomSelected;
            set
            {
                Set(ref _isCustomSelected, value);
                if (_isCustomSelected)
                    Tab = PaletteTab.Custom;
            }
        }
        public bool IsCsvSelected
        {
            get => _isCsvSelected;
            set
            {
                Set(ref _isCsvSelected, value);
                if (_isCsvSelected)
                    Tab = PaletteTab.Csv;
            }
        }
        public bool IsMassSelected
        {
            get => _isMassSelected;
            set
            {
                Set(ref _isMassSelected, value);
                if(_isMassSelected)
                    Tab = PaletteTab.Mass;
            }
        }

        public double Mass
        {
            get => _mass;
            set => Set(ref _mass, value);
        }

        public void RefreshHexCount()
        {
            RaisePropertyChangedEvent(nameof(CsvHexCount));
        }

        public void CreateHexes()
        {
            double mass = Mass;
            if(MassHexes.LastOrDefault(h => h.ColorPatterns.Count > 1) is CsvHexPickerVM hex)
            {
                var existing = hex.ColorPatterns.First();
                var existingCount = existing.Count;
                mass -= (21 - existingCount);

                MassHexes.Remove(hex);
                var newHex = new CsvHexPickerVM
                {
                    ColorPatterns = new ObservableCollection<ColorPattern>
                    {
                        new ColorPattern(existingCount, new SolidColorBrush(existing.Color.Color)),
                        new ColorPattern(21-existingCount, new SolidColorBrush(MassBrush.Color))
                    }
                };
                MassHexes.Add(newHex);
            }

            double i;
            for (i = 10.5; i <= mass; i+=10.5)
            {
                MassHexes.Add(new CsvHexPickerVM
                {
                    ColorPatterns = new ObservableCollection<ColorPattern>
                    {
                        new ColorPattern(1, new SolidColorBrush(MassBrush.Color))
                    }
                });
            }

            if (mass % 10.5 != 0)
            {
                int remaining = 2 * (int)(10.5 + mass - i);
                MassHexes.Add(new CsvHexPickerVM
                {
                    ColorPatterns = new ObservableCollection<ColorPattern>
                    {
                        new ColorPattern(remaining, new SolidColorBrush(MassBrush.Color)),
                        new ColorPattern(21-remaining, Brushes.Transparent)
                    }
                });
            }
        }
    }
}
