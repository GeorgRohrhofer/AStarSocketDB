using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [Serializable]
    internal class Node
    {
        public string Desc { get; set; }   
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsMarked { get; set; }

        private Dictionary<Node, int> neighbours;

        public static readonly int SIZE = 16;
        public static readonly int SIZE_DISTANCE = 10;

        public Node(string desc, int x, int y)
        {
            Desc = desc;
            X = x;
            Y = y;
            neighbours = new Dictionary<Node, int>();
        }

        public bool IsInNode(int x, int y)
        {
            return SIZE > Distance(x, y);
        }

        public Node IsInWeight(int x, int y)
        {
            int posX;
            int posY;
            foreach(Node n in neighbours.Keys)
            {
                posX = X + (n.X - X) / 2;
                posY = Y + (n.Y - Y) / 2;

                if((posX-SIZE_DISTANCE < x) && (x < posX + SIZE_DISTANCE) && (posY - SIZE_DISTANCE < x) && (y < posY + SIZE_DISTANCE))
                {
                    return n;
                }
            }
            return null;
        }

        public void Increment(Node n)
        {
            neighbours[n] = neighbours[n] + 1;
        }

        public void Decrement(Node n)
        {
            int dist = (int)this.Distance(n.X, n.Y);
            if (neighbours[n] > dist)
            {
                neighbours[n] = neighbours[n] - 1;
            }
        }

        public bool IsNearNode(int x, int y)
        {
            return SIZE*2 > Distance(x, y);
        }

        public double Distance(int x, int y)
        {
            int dx = X - x;
            int dy = Y - y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public void Move(int dx, int dy)
        {
            X += dx;
            Y += dy;

            foreach(Node n in neighbours.Keys.ToList())
            {
                neighbours[n] = (int)Distance(n.X, n.Y);
                n.neighbours[this] = (int)Distance(n.X, n.Y);
            }
        }

        public bool AddNeighbour(Node n, int distance)
        {
            if (neighbours.ContainsKey(n))
                return false;

            neighbours.Add(n, distance);
            return true;
        }

        public Dictionary<Node, int> GetNeighbours()
        {
            return neighbours;
        }

        public int GetDistanceTo(Node neighbour)
        {
            return neighbours[neighbour];
        }

        public void Print()
        {
            Console.WriteLine(Desc);
            if (neighbours.Count != 0)
            {
                foreach (Node n in neighbours.Keys)
                {
                    Console.WriteLine("    " + n.Desc + ": " + neighbours[n]);
                }
            }
            else
            {
                Console.WriteLine("    No neighbours");
            }
        }
    }
}
