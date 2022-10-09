using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Quilt
{
    /// <summary>
    /// Interaction logic for QuiltLayout.xaml
    /// </summary>
    public partial class QuiltLayout : UserControl
    {
        public QuiltLayout()
        {
            InitializeComponent();
        }

        private void CreateQuilt()
        {
            if (DataContext is MainVM vm)
            {
                QuiltCanvas.Children.Clear();
                foreach (HexVM hex in vm.HexList)
                {
                    var newHex = new HexTile
                    {
                        Hex = hex
                    };

                    Canvas.SetTop(newHex, 75 * hex.Y);

                    if (hex.Y % 2 == 0)
                    {
                        Canvas.SetLeft(newHex, 86 * hex.X);
                    }
                    else
                    {
                        Canvas.SetLeft(newHex, 43 + 86 * hex.X);
                    }
                    QuiltCanvas.Children.Add(newHex);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainVM vm)
            {
                vm.GenerateRandomQuilt();
            }
        }

        private void Seeded_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainVM vm)
            {
                vm.GenerateRandomQuilt(true);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainVM vm)
            {
                vm.GenerateRandomQuilt(fillColors:false);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is MainVM vm && sender is QuiltLayout quilt)
            {
                vm.PropertyChanged += (s,o) => OnPropertyChanged(s,o);
                vm.GenerateRandomQuilt(false, false);
            }
        }

        private void OnPropertyChanged(object s, PropertyChangedEventArgs o)
        {
            if(o.PropertyName == nameof(MainVM.HexList))
            {
                CreateQuilt();
            }
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is MainVM vm && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) 
            {
                vm.Config.Zoom *= Math.Pow(2, e.Delta / 120);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainVM vm)
            {
                vm.Tabs.HexPatternPickers.Add(new HexPatternPickerVM());
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainVM vm && sender is HexPatternPicker picker)
            {
                vm.Tabs.HexPatternPickers.Remove((HexPatternPickerVM)picker.DataContext);
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainVM vm)
            {
                vm.Tabs.CreateHexes();
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainVM vm)
            {
                CsvUtil.SavePatternsToCsv(new List<List<HexPatternPickerVM>> { new List<HexPatternPickerVM>(vm.Tabs.HexPatternPickers) });
                vm.GetFromCsv();
                vm.GenerateRandomQuilt(fillColors: false);
            }
        }
    }
}
