using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [Serializable]
    internal class NodeManagement
    {
        private Dictionary<string, Node> nodes;
        private Node startNode, endNode;
        public static List<Node> resultPath = new List<Node>();
        private Socket socket_conn;

        public Socket Socket_Conn
        {
            set { socket_conn = value; }
            private get { return socket_conn; }
        }

        public Node StartNode
        {
            get { return startNode; }
            set
            {
                if(startNode != null)
                {
                    startNode.IsMarked = false;
                }
                startNode = value;
                if(startNode != null)
                {
                    startNode.IsMarked = true;
                }
            }
        }

        public Node EndNode
        {
            get { return endNode; }
            set
            {
                if (endNode != null)
                {
                    endNode.IsMarked = false;
                }
                endNode = value;
                if (endNode != null)
                {
                    endNode.IsMarked = true;
                }
            }
        }

        public NodeManagement()
        {
            nodes = new Dictionary<string, Node>();
        }

        public void Add(int x, int y)
        {
            nodes.Add((nodes.Count + 1) + "", new Node((nodes.Count + 1)+"", x, y));

            string msg;
            byte[] msgByte;
            msg = "addPoint(" + (nodes.Count+1) + "," + x + "," + y + ")";
        }

        public void Add(string desc, int x, int y)
        {
            nodes.Add(desc + "", new Node((nodes.Count + 1) + "", x, y));

            string msg;
            byte[] msgByte;
            msg = "addPoint(" + (nodes.Count + 1) + "," + x + "," + y + ")";
        }

        public Node IsInNode(int x, int y)
        {
            foreach (Node n in nodes.Values)
            {
                if (n.IsInNode(x, y))
                {
                    return n;
                }
            }
            return null;
        }

        public List<Node> IsInWeight(int x, int y)
        {
            List<Node> result = new List<Node>();
            int posX;
            int posY;
            foreach (Node n in nodes.Values)
            {
                if(n.IsInWeight(x, y) != null)
                {
                    result.Add(n);
                    result.Add(n.IsInWeight(x, y));
                    return result;
                }
            }
            return result;
        }

        public void IncrementWeight(string desc1, string desc2)
        {
            nodes[desc1].Increment(nodes[desc2]);
            nodes[desc2].Increment(nodes[desc1]);
        }

        public void DecrementWeight(string desc1, string desc2)
        {
            nodes[desc1].Decrement(nodes[desc2]);
            nodes[desc2].Decrement(nodes[desc1]);
        }

        public bool IsNearNode(int x, int y)
        {
            foreach (Node n in nodes.Values)
            {
                if (n.IsNearNode(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> Search(string snode, string enode)
        {
            startNode = nodes[snode];
            endNode = nodes[enode];
            ResetMarked();
            OpenList openList = new OpenList();
            ClosedList closedList = new ClosedList();

            ListEntry entry = new ListEntry(startNode, 0, (int)startNode.Distance(endNode.X, endNode.Y) ,null);
            openList.Add(entry);
            entry = openList.GetBest();

            while ((entry != null))
            {
                foreach(Node n in entry.NodeEntry.GetNeighbours().Keys)
                {
                    if (closedList.IsInClosed(n))
                    {
                        
                    }
                    else if (!openList.IsInOpen(n))
                    {
                        ListEntry newEntry = new ListEntry(n, entry.Distance + entry.NodeEntry.GetDistanceTo(n), (int)n.Distance(endNode.X, endNode.Y),entry.NodeEntry);
                        openList.Add(newEntry);
                    }
                    else
                    {
                        ListEntry e = openList.Get(n);
                        if(entry.Distance + entry.NodeEntry.GetDistanceTo(n) < e.Distance)
                        {
                            e.Distance = entry.Distance + entry.NodeEntry.GetDistanceTo(n);
                            e.Predecessor = entry.NodeEntry;
                        }
                    }
                }
                closedList.Add(entry);
                entry = openList.GetBest();
            }

            List<Node> result = closedList.GetResult(EndNode);
            resultPath = new List<Node>();
            List<string> resultPathstring = new List<string>();
            foreach(Node n in result)
            {
                n.IsMarked = true;
                resultPath.Add(n);
            }
            foreach(Node n in resultPath)
            {
                resultPathstring.Add(n.Desc);
            }

            return resultPathstring;
        }

        public void ResetMarked()
        {
            foreach(Node n in nodes.Values)
            {
                n.IsMarked = false;
            }
        }

        public void Print()
        {
            foreach(Node n in nodes.Values)
            {
                n.Print();
            }
        }

        public bool AddConnection(string n1, string n2, int dist)
        {
            if(nodes.ContainsKey(n1) && nodes.ContainsKey(n2))
            {
                return nodes[n1].AddNeighbour(nodes[n2], dist) && nodes[n2].AddNeighbour(nodes[n1], dist);
            }

            return false;
        }
    }
}
