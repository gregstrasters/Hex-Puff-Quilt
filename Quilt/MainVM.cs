using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Quilt
{
    public class MainVM : BaseVM
    {
        private static List<ColorPattern> s_unselectedPattern = new List<ColorPattern>
            {
                new ColorPattern(1, Brushes.Black),
                new ColorPattern(1, Brushes.Yellow),
                new ColorPattern(1, Brushes.Black),
                new ColorPattern(1, Brushes.Yellow),
                new ColorPattern(1, Brushes.Black)
            };
        private static List<ColorPattern> s_unfilledPattern = new List<ColorPattern> { new ColorPattern(1, Brushes.White) };

        private ObservableCollection<HexVM> _hexList = new ObservableCollection<HexVM>();
        private Network _network;

        public QuiltConfig Config { get; set; } = new QuiltConfig();
        public PaletteTabs Tabs { get; set; } = new PaletteTabs();

        public ObservableCollection<HexVM> HexList
        {
            get => _hexList;
            set => Set(ref _hexList, value);
        }


        private void OnCsvHexPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(CsvHexPickerVM.IsSelected) && sender is CsvHexPickerVM csvHex && csvHex.IsSelected)
            {
                foreach(var other in Tabs.CsvHexes.SelectMany(l => l.Patterns).Where(h => !h.Equals(csvHex) && h.IsSelected))
                {
                    other.IsSelected = false;
                }

                foreach (var other in HexList.Where(h => h.IsSelected))
                {
                    other.IsSelected = false;
                }
                if(Tabs.IsFillRegionSelected)
                {
                    Tabs.IsFillRegionSelected = false;
                }
            }
        }

        private void RandomlySortColors(Random rand)
        {
            Tabs.CsvColors.ForEach(l => l.Sort((a, b) => rand.Next(0, 4) - 2));
        }

        public void GetFromCsv()
        {
            Tabs.CsvColors = CsvUtil.GetPatternsFromCSV();
        }

        public void GenerateRandomQuilt(bool useSeed = false, bool fillColors = true)
        {
            Tabs.CsvHexes.Clear();
            foreach (var patternLists in Tabs.CsvColors)
            {
                var group = new PatternGroupVM();
                foreach (var patterns in patternLists)
                {
                    var csvHex = new CsvHexPickerVM
                    {
                        ColorPatterns = new ObservableCollection<ColorPattern>(patterns)
                    };
                    group.Patterns.Add(csvHex);
                    csvHex.PropertyChanged += OnCsvHexPropertyChanged;
                }
                Tabs.CsvHexes.Add(group);
            }

            var rand = useSeed ? new Random(Config.Seed) : new Random();
            RandomlySortColors(rand);
            _network = new Network(new Random(rand.Next()));
            GenerateQuilt(fillColors);
        }

        private void GenerateQuilt(bool fillColors)
        {
            List<HexVM> hexes = new List<HexVM>();
            int regionSize = 16;
            _network.CreateNetwork(Config.ColumnCount, Config.RowCount);
            _network.ClaimAllNodes((int)Math.Log(regionSize, 2));
            var regions = _network.Regions.Where(r => r.Any());
            foreach (var region in regions)
            {
                var regionIndex = _network.Regions.IndexOf(region);
                var patterns = Tabs.CsvColors[regionIndex % Tabs.CsvColors.Count()];
                hexes.AddRange(FillRegionWithPatterns(region.Id, patterns, fillColors));
            }

            HexList = new ObservableCollection<HexVM>(hexes);
        }

        public List<HexVM> FillRegionWithPatterns(int regionId, List<List<ColorPattern>> patterns, bool fillColors = true)
        {
            var hexes = new List<HexVM>();
            var region = _network.Regions.Single(r => r.Id == regionId);
            foreach (var node in region)
            {
                var index = region.IndexOf(node);
                HexVM newHex = null;
                if (index < patterns.Count())
                {
                    newHex = new HexVM(node, fillColors ? patterns[index] : s_unfilledPattern);
                }
                else
                {
                    newHex = new HexVM(node, fillColors ? s_unselectedPattern : s_unfilledPattern, 60);
                }
                newHex.PropertyChanged += OnHexChanged;
                hexes.Add(newHex);
            }
            return hexes;
        }

        public void FillRegion(int regionId)
        {
            var rand = new Random();
            var region = _network.Regions.Single(r => r.Id == regionId);
            var pickers = Tabs.HexPatternPickers.Where(p => p.IsSelected).ToList();
            if (!pickers.Any())
                return;

            foreach (var hex in HexList.Where(h => !h.IgnoreOverall && h.Node.RegionId == regionId))
            {
                var index = rand.Next(0, pickers.Count);
                hex.ColorPatterns = pickers[index].GetColorPatterns();
                hex.Rotation = pickers[index].Rotation;
            }
        }

        private void OnHexChanged(object s, PropertyChangedEventArgs e)
        {
            if (s is HexVM hex && e.PropertyName == nameof(HexVM.IsSelected) && hex.IsSelected)
            {
                if (HexList.FirstOrDefault(h => !h.Equals(hex) && h.IsSelected) is HexVM otherHex)
                {
                    HexVM.Swap(hex, otherHex);
                }
                else if (Tabs.CsvHexes.SelectMany(l => l.Patterns).FirstOrDefault(h => h.IsSelected) is CsvHexPickerVM csvHex)
                {
                    FillFromCsv(hex, csvHex);
                    hex.IsSelected = false;
                }
                else if (Tabs.MassHexes.FirstOrDefault(h => h.IsSelected) is CsvHexPickerVM massHex)
                {
                    hex.ColorPatterns = massHex.ColorPatterns.ToList();
                    hex.Rotation = massHex.Rotation;
                    hex.IgnoreOverall = true;
                    Tabs.MassHexes.Remove(massHex);
                    hex.IsSelected = false;
                }
                else
                {
                    switch (Tabs.Tab)
                    {
                        case PaletteTab.Custom:
                            if (Tabs.IsFillRegionSelected)
                            {
                                FillRegion(hex.Node.RegionId);
                                hex.IsSelected = false;
                            }
                            break;
                        case PaletteTab.Csv:
                            if (Tabs.IsCsvFillRegionSelected)
                            {
                                var rand = new Random();
                                var hexes = Tabs.CsvHexes.Where(g => g.IsSelected).SelectMany(g => g.Patterns).ToList();
                                hexes.Sort((a, b) => rand.Next(0, 2) - 1);
                                var regionHexes = HexList.Where(h => h.Node.RegionId == hex.Node.RegionId
                                    && (h.ColorPatterns == null || !h.ColorPatterns.Any() || h.ColorPatterns == s_unfilledPattern)).ToList();
                                foreach (var csv in hexes)
                                {
                                    int index = hexes.IndexOf(csv);
                                    if (index >= regionHexes.Count)
                                        break;

                                    var regionHex = regionHexes[index];
                                    FillFromCsv(regionHex, csv);
                                }
                                RaisePropertyChangedEvent(nameof(HexList));
                                hex.IsSelected = false;
                            }
                            break;
                        case PaletteTab.Mass:
                            if (Tabs.IsMassFillRegionSelected)
                            {

                            }
                            break;
                    }
                }
            }
        }

        private void FillFromCsv(HexVM hex, CsvHexPickerVM csvHex)
        {
            hex.ColorPatterns = csvHex.ColorPatterns.ToList();
            hex.Rotation = csvHex.Rotation;
            hex.IgnoreOverall = true;
            Tabs.CsvHexes.Single(l => l.Patterns.Contains(csvHex)).Patterns.Remove(csvHex);
        }
    }
}
