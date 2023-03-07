using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStarSocketDB
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
        public static readonly Font FONT = new Font("Arial", 12);
        public static readonly Font FONT_DISTANCE = new Font("Arial", 8);
        public static readonly Brush BACKBRUSH = new SolidBrush(Color.FromArgb(240, 240, 240));

        public Node(string desc, int x, int y)
        {
            Desc = desc;
            X = x;
            Y = y;
            neighbours = new Dictionary<Node, int>();
        }

        public void Paint(Graphics g)
        {
            Pen p = Pens.Black;
            Brush b = new SolidBrush(Color.Black);

            if (IsMarked)
            {
                p = Pens.Red;
                b = new SolidBrush(Color.Red);
            }

            g.FillEllipse(BACKBRUSH, X - SIZE, Y - SIZE, SIZE * 2, SIZE * 2);
            g.DrawEllipse(p, X - SIZE, Y - SIZE, SIZE * 2, SIZE * 2);
            SizeF dim = g.MeasureString(Desc, FONT);
            g.DrawString(Desc, FONT, b, X - dim.Width / 2, Y - dim.Height / 2);
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

        public void PaintNeighbours(Graphics g)
        {
            int posX, posY;
            SizeF dim;

            Pen p = Pens.Black;
            Brush b = new SolidBrush(Color.Black);

            foreach (Node n in neighbours.Keys)
            {
                p = Pens.Black;
                b = new SolidBrush(Color.Black);

                if ((IsMarked) && (n.IsMarked) && (Math.Abs(NodeManagement.resultPath.IndexOf(this) - NodeManagement.resultPath.IndexOf(n)) == 1))
                {
                    p = Pens.Red;
                    b = new SolidBrush(Color.Red);
                }

                if (Desc.CompareTo(n.Desc) <= 0)
                {
                    posX = X + (n.X - X) / 2;
                    posY = Y + (n.Y - Y) / 2;

                    g.DrawLine(p, X, Y, n.X, n.Y);

                    g.FillRectangle(BACKBRUSH, posX - SIZE_DISTANCE, posY - SIZE_DISTANCE, SIZE_DISTANCE * 2, SIZE_DISTANCE * 2);
                    g.DrawRectangle(p, posX - SIZE_DISTANCE, posY - SIZE_DISTANCE, SIZE_DISTANCE * 2, SIZE_DISTANCE * 2);

                    dim = g.MeasureString(neighbours[n] + "", FONT_DISTANCE);

                    g.DrawString(neighbours[n] + "", FONT_DISTANCE, b, posX - dim.Width / 2, posY - dim.Height / 2);
                }
            }
        }

        public Dictionary<Node, int> GetNeighbours()
        {
            return neighbours;
        }

        public int GetDistanceTo(Node neighbour)
        {
            return neighbours[neighbour];
        }
    }
}
