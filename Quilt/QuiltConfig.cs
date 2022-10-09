using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quilt
{
    public class QuiltConfig : BaseVM
    {
        private int _columnCount = 20;
        private int _rowCount = 14;
        private double _zoom = .5;
        private int _seed = 0;

        public int ColumnCount
        {
            get => _columnCount;
            set => Set(ref _columnCount, value, addtlPropertyChanges: new[] { nameof(Width) });
        }

        public int RowCount
        {
            get => _rowCount;
            set => Set(ref _rowCount, value, addtlPropertyChanges: new[] { nameof(Height) });
        }
        public int Width => ColumnCount * 86 + 43;
        public int Height => RowCount * 75;
        public double Zoom
        {
            get => _zoom;
            set => Set(ref _zoom, value);
        }
        public int Seed
        {
            get => _seed;
            set => Set(ref _seed, value);
        }
    }
}
