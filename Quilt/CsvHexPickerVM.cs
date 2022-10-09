using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quilt
{
    public class CsvHexPickerVM : BaseVM
    {
        private bool _isSelected;
        private ObservableCollection<ColorPattern> _colorPatterns;
        private int _rotation;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }
        public ObservableCollection<ColorPattern> ColorPatterns
        {
            get { return _colorPatterns; }
            set { Set(ref _colorPatterns, value); }
        }
        public int Rotation
        {
            get { return _rotation; }
            set { Set(ref _rotation, value); }
        }
    }
}
