using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace AStarSocketDB
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
            msg = "addPoint(" + (nodes.Count) + "," + x + "," + y + ")";

            msgByte = Encoding.UTF8.GetBytes(msg);
            Socket_Conn.Send(msgByte);
        }

        public void Paint(Graphics g)
        {
            foreach (Node n in nodes.Values)
            {
                n.PaintNeighbours(g);
            }

            foreach (Node n in nodes.Values)
            {
                n.Paint(g);
            }
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

        public void Search()
        {
            ResetMarked();

            string msg;
            byte[] msgByte;
            msg = "search(" + StartNode.Desc + "," + EndNode.Desc + ")";

            msgByte = Encoding.UTF8.GetBytes(msg);
            Socket_Conn.Send(msgByte);

            byte[] bytes = new byte[8192];
            int bytesRecieved = socket_conn.Receive(bytes);
            msg = Encoding.ASCII.GetString(bytes, 0, bytesRecieved);

            string[] parts = msg.Split(',');

            foreach(string s in parts)
            {
                if (s != null && nodes.ContainsKey(s))
                    resultPath.Add(nodes[s]);
                    nodes[s].IsMarked = true;
            }
            
        }

        public void ResetMarked()
        {
            foreach(Node n in nodes.Values)
            {
                n.IsMarked = false;
            }
        }

        public bool AddConnection(string n1, string n2, int dist)
        {
            if (nodes.ContainsKey(n1) && nodes.ContainsKey(n2))
            {
                string msg;
                byte[] msgByte;
                msg = "addConnection(" + n1 + "," + n2 + "," + dist + ")";

                msgByte = Encoding.UTF8.GetBytes(msg);
                Socket_Conn.Send(msgByte);
                

                return nodes[n1].AddNeighbour(nodes[n2], dist) && nodes[n2].AddNeighbour(nodes[n1], dist);
            }
            return false;   

        }

        public void send()
        {
            foreach(string s in nodes.Keys)
            {
                string msg = "";
                byte[] msgByte;
                msg = "addPoint(" + nodes[s].Desc + "," + nodes[s].X + "," + nodes[s].Y + ")";

                msgByte = Encoding.UTF8.GetBytes(msg);
                Socket_Conn.Send(msgByte);
                Thread.Sleep(1);
            }

            foreach(string s in nodes.Keys)
            {
                string msg;
                byte[] msgByte;
                foreach(Node ne in nodes[s].GetNeighbours().Keys)
                {
                    msg = "addConnection(" + ne.Desc + "," + nodes[s].Desc + "," + nodes[s].GetNeighbours()[ne] + ")";

                    msgByte = Encoding.UTF8.GetBytes(msg);
                    Socket_Conn.Send(msgByte);
                    Thread.Sleep(1);
                }
            }
        }
    }
}
