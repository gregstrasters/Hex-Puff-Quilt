using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quilt
{
    public class PatternGroupVM : BaseVM
    {
        private bool _isSelected;
        private ObservableCollection<CsvHexPickerVM> _patterns = new ObservableCollection<CsvHexPickerVM>();

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
        public ObservableCollection<CsvHexPickerVM> Patterns
        {
            get => _patterns;
            set => Set(ref _patterns, value);
        }
    }
}
