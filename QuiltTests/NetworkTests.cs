using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quilt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quilt.Tests
{
    [TestClass()]
    public class NetworkTests
    {
        [TestMethod()]
        public void CreateNetworkTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        //public void JustTest()
        //{
        //    var network = new Network();
        //    network.InitRegions(10, 5);
        //    network.CreateNetwork(7, 8);
        //    network.SetStartingNodes();
        //    foreach (var region in network.Regions)
        //    {
        //        Console.WriteLine($"{network.Regions.IndexOf(region)}: {region[0].Id}");
        //        network.ClaimNodes(region);
        //        Console.WriteLine(region.Aggregate("", (str,n) => str + "," + n.Id));
        //    }

        //}

        [TestMethod()]
        public void BetterTest()
        {
            var network = new Network(new Random(0));
            network.CreateNetwork(24, 33);
            network.ClaimAllNodes(3);
        }

        //[TestMethod]
        //public void GetEdgeNodes_ShouldReturnNodesWithUnclaimedNeighbors_ForListOfNodes()
        //{
        //    var network = new Network();
        //    network.InitRegions(5, 5);
        //    network.CreateNetwork(5, 5);
        //    network.SetStartingNodes();
        //    Assert.AreEqual(1, network.GetEdgeNodes(network.Regions[0]).Count());
        //    Assert.AreEqual(network.Regions[0][0], network.GetEdgeNodes(network.Regions[0])[0]);
        //}

        //[TestMethod]
        //public void ClaimNodes_ShouldClaimOneNode()
        //{

        //}

        //[TestMethod]
        //public void ClaimNodes_ShouldNotClaimAnyAlreadyClaimedNodes()
        //{

        //}

        //[TestMethod]
        //public void CheckForOrphans_ShouldReturnNodes_IfAnyNodeWasOrphaned()
        //{

        //}

        //[TestMethod]
        //public void SetStartingNodes_ShouldClaimOneNodePerRegion()
        //{

        //}

        //[TestMethod]
        //public void CreateNetwork_ShouldGenerateNodes()
        //{

        //}
    }
}