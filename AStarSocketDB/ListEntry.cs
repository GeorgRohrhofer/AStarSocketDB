using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarSocketDB
{
    internal class ListEntry
    {
        public Node NodeEntry { get; set; }
        public int Distance { get; set; }
        public int s { set; get; }
        public Node Predecessor { get; set; }

        public ListEntry(Node nodeEntry, int distance, int s, Node predecessor)
        {
            NodeEntry = nodeEntry;
            Distance = distance;
            Predecessor = predecessor;
            this.s = s;
        }
    }
}
