using System;
using System.Collections.Generic;
using System.Linq;

namespace Quilt
{
    public class Network
    {
        private int _columnCount;
        private int _rowCount;
        public int RegionSize { get; set; }
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Region> Regions { get; set; } = new List<Region>();
        private Random _rand;

        public Network(Random rand)
        {
            _rand = rand;
        }

        public void CreateNetwork(int columnCount, int rowCount)
        {
            _columnCount = columnCount;
            _rowCount = rowCount;

            for (int i = 0; i < rowCount; i++)
            {
                for(int j = 0; j < columnCount; j++)
                {
                    var node = new Node { X = j, Y = i };
                    Nodes.Add(node);
                }
            }

            foreach(var node in Nodes)
            {
                SetNeighbors(node);
            }
        }

        public void SetNeighbors(Node node)
        {
            node.LeftNeighbor = Nodes.FirstOrDefault(n => n.X == node.X - 1 && n.Y == node.Y);
            node.RightNeighbor = Nodes.FirstOrDefault(n => n.X == node.X + 1 && n.Y == node.Y);

            if (node.Y % 2 == 0)
            {
                node.TopLeftNeighbor = Nodes.FirstOrDefault(n => n.X == node.X - 1 && n.Y == node.Y - 1);
                node.TopRightNeighbor = Nodes.FirstOrDefault(n => n.X == node.X && n.Y == node.Y - 1);
                node.BottomLeftNeighbor = Nodes.FirstOrDefault(n => n.X == node.X - 1 && n.Y == node.Y + 1);
                node.BottomRightNeighbor = Nodes.FirstOrDefault(n => n.X == node.X && n.Y == node.Y + 1);
            }
            else
            {
                node.TopLeftNeighbor = Nodes.FirstOrDefault(n => n.X == node.X && n.Y == node.Y - 1);
                node.TopRightNeighbor = Nodes.FirstOrDefault(n => n.X == node.X + 1 && n.Y == node.Y - 1);
                node.BottomLeftNeighbor = Nodes.FirstOrDefault(n => n.X == node.X && n.Y == node.Y + 1);
                node.BottomRightNeighbor = Nodes.FirstOrDefault(n => n.X == node.X + 1 && n.Y == node.Y + 1);
            }
        }

        public List<Node> GetEdgeNodes(List<Node> region, List<Node> availableNodes)
        {
            return region.Where(n => n.Neighbors.Any(nb => availableNodes.Contains(nb))).ToList();
        }

        public void ClaimAllNodes(int iterationCount)
        {
            var avaliableNodes = Nodes.ToList();
            Regions = new List<Region>();
            foreach(var node in Nodes)
            {
                if (!avaliableNodes.Contains(node))
                    continue;

                avaliableNodes.Remove(node);

                var neighbor = GetRandomViableNeighbor(node, avaliableNodes);
                if (neighbor == null)
                {
                    continue;
                }

                var newRegion = new Region { node, neighbor };
                Regions.Add(newRegion);
                node.RegionId = newRegion.Id;
                neighbor.RegionId = newRegion.Id;
                avaliableNodes.Remove(neighbor);
            }

            for (int i = 1; i < iterationCount; i++)
            {
                MergeRegions();
            }

            foreach (var node in Nodes.Where(n => !Regions.Any(r => r.Contains(n))))
            {
                var index = _rand.Next(0, node.Neighbors.Count());
                var neighbor = node.Neighbors[index];
                var neighborRegion = Regions.FirstOrDefault(r => r.Contains(neighbor));
                neighborRegion.Add(node);
                node.RegionId = neighborRegion.Id;
            }
        }

        private void MergeRegions()
        {
            var avaliableNodes = Nodes.ToList();
            foreach (var region in Regions)
            {
                if (!region.Any())
                    continue;

                avaliableNodes.RemoveAll(n => region.Contains(n));
                var edges = GetEdgeNodes(region, avaliableNodes);
                if (!edges.Any())
                {
                    continue;
                }
                var index = _rand.Next(0, edges.Count());
                var edge = edges[index];
                var neighbor = GetRandomViableNeighbor(edge, avaliableNodes);
                var neighborRegion = Regions.FirstOrDefault(r => r.Contains(neighbor));
                if (neighborRegion == null)
                {
                    region.Add(neighbor);
                    neighbor.RegionId = region.Id;
                    avaliableNodes.Remove(neighbor);
                    continue;
                }

                region.AddRange(neighborRegion);
                neighborRegion.ForEach(r => r.RegionId = region.Id);
                avaliableNodes.RemoveAll(n => neighborRegion.Contains(n));
                neighborRegion.Clear();
            }
        }

        private Node GetRandomViableNeighbor(Node node, List<Node> availableNodes)
        {
            var viableNeighbors = node.Neighbors.Where(n => availableNodes.Contains(n)).ToList();
            if (!viableNeighbors.Any())
            {
                return null;
            }
            var index = _rand.Next(0, viableNeighbors.Count());
            return viableNeighbors[index];
        }
    }
}
