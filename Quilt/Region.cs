using System.Collections.Generic;

namespace Quilt
{
    public class Region : List<Node>
    {
        private static int s_latestId = 0;

        public int Id { get; set; }

        public Region()
        {
            Id = s_latestId++;
        }
    }
}
