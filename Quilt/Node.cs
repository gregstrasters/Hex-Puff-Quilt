using System.Collections.Generic;
using System.Linq;

namespace Quilt
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int RegionId { get; set; }

        public Node TopLeftNeighbor { get; set; }
        public Node LeftNeighbor { get; set; }
        public Node BottomLeftNeighbor { get; set; }
        public Node TopRightNeighbor { get; set; }
        public Node RightNeighbor { get; set; }
        public Node BottomRightNeighbor { get; set; }

        public List<Node> Neighbors => new List<Node>
        {
            TopLeftNeighbor,
            LeftNeighbor,
            BottomLeftNeighbor,
            TopRightNeighbor,
            RightNeighbor,
            BottomRightNeighbor
        }.Where(n => n != null).ToList();
    }
}
