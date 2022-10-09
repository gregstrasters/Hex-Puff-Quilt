using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Quilt
{
    public struct ColorPattern
    {
        public int Count;
        public SolidColorBrush Color;

        public ColorPattern(int count, SolidColorBrush color)
        {
            Count = count;
            Color = color;
        }
    }

    public struct HexBorder
    {
        public bool TopLeft;
        public bool Left;
        public bool BottomLeft;
        public bool TopRight;
        public bool Right;
        public bool BottomRight;

        public HexBorder(bool topLeft = true, bool left = true, bool bottomLeft = true, bool topRight = true, bool right = true, bool bottomRight = true)
        {
            TopLeft = topLeft;
            Left = left;
            BottomLeft = bottomLeft;
            TopRight = topRight;
            Right = right;
            BottomRight = bottomRight;
        }
    }

    public class HexVM : BaseVM
    {
        private static int s_lastId = 0;
        public int Id;
        public int X => Node.X;
        public int Y => Node.Y;

        private int _rotation;
        public int Rotation
        {
            get => _rotation;
            set
            {
                Set(ref _rotation, value);
            }
        }

        private List<ColorPattern> _colorPatterns;
        public List<ColorPattern> ColorPatterns
        {
            get { return _colorPatterns; }
            set { Set(ref _colorPatterns, value); }
        }

        private bool _ignoreOverall;
        public bool IgnoreOverall
        {
            get { return _ignoreOverall; }
            set { Set(ref _ignoreOverall, value); }
        }

    private Node _node;
        public Node Node
        {
            get => _node;
            set 
            {
                Set(ref _node, value, addtlPropertyChanges:new []{ nameof(X), nameof(Y)});
                UpdateBorder();
            }
        }

        private HexBorder _hexBorder;
        public HexBorder Border
        {
            get => _hexBorder;
            set => Set(ref _hexBorder, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public HexVM(Node node, List<ColorPattern> colors, int rotation = 0)
        {
            Id = s_lastId++;
            ColorPatterns = colors;
            Rotation = rotation;
            Node = node;
            UpdateBorder();
        }

        public HexVM(Node node, SolidColorBrush color, int rotation = 0)
        {
            Id = s_lastId++;
            ColorPatterns = new List<ColorPattern> { new ColorPattern(1, color) };
            Rotation = rotation;
            Node = node;
            UpdateBorder();
        }

        public void UpdateBorder()
        {
            Border = new HexBorder
            {
                TopLeft = Node?.TopLeftNeighbor?.RegionId != Node.RegionId,
                Left = Node?.LeftNeighbor?.RegionId != Node.RegionId,
                BottomLeft = Node?.BottomLeftNeighbor?.RegionId != Node.RegionId,
                TopRight = Node?.TopRightNeighbor?.RegionId != Node.RegionId,
                Right = Node?.RightNeighbor?.RegionId != Node.RegionId,
                BottomRight = Node?.BottomRightNeighbor?.RegionId != Node.RegionId
            };
        }

        public static void Swap(HexVM hex, HexVM otherHex)
        {
            var patterns = hex.ColorPatterns;
            var rotation = hex.Rotation;
            var ignore = hex.IgnoreOverall;
            hex.ColorPatterns = otherHex.ColorPatterns;
            hex.Rotation = otherHex.Rotation;
            hex.IgnoreOverall = otherHex.IgnoreOverall;
            otherHex.ColorPatterns = patterns;
            otherHex.Rotation = rotation;
            otherHex.IgnoreOverall = ignore;
            otherHex.IsSelected = false;
            hex.IsSelected = false;
        }
    }
}
