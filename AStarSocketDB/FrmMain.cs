using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStarSocketDB
{

    public partial class FrmMain : Form
    {
        private Socket socket_sender;
        private NodeManagement nodes;
        private Node actNode;
        private bool moving = false;
        private bool connecting = false;
        private int oldX, oldY;

        public FrmMain()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            nodes = new NodeManagement();
            /*
            FileStream fs = new FileStream("C:\\Temp\\temp.bin", FileMode.Open);

            BinaryFormatter bf = new BinaryFormatter();
            nodes = (NodeManagement)bf.Deserialize(fs);
            Invalidate();*/
            miDisconnect.Enabled = false;
        }

        private void FrmMain_MouseDown(object sender, MouseEventArgs e)
        {
            actNode = nodes.IsInNode(e.X, e.Y);
            List<Node> inWeight = nodes.IsInWeight(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                if(inWeight.Count > 0)
                {
                    
                    nodes.DecrementWeight(inWeight[0].Desc, inWeight[1].Desc);

                    if (nodes.StartNode != null && nodes.EndNode != null)
                    {
                        nodes.Search();
                    }
                }

                else if((actNode == null) && !nodes.IsNearNode(e.X, e.Y))
                    nodes.Add(e.X, e.Y);
                else if((actNode != null)&&(Control.ModifierKeys == Keys.Control))
                {
                    moving = true;
                }
                else if (actNode != null)
                {
                    connecting = true;
                }
                Invalidate();
            }
            else if((e.Button == MouseButtons.Right) && (actNode != null))
            {
                if ((nodes.StartNode == null) && (nodes.EndNode == null))
                {
                    miSetEndNode.Enabled = false;
                    miResetStartEndNode.Enabled = false;
                }
                else
                {
                    miSetEndNode.Enabled = true;
                    miResetStartEndNode.Enabled = true;
                }

                cmEditNode.Show(this, e.X, e.Y);
            }

            else if (inWeight.Count > 0)
            {
                nodes.IncrementWeight(inWeight[0].Desc, inWeight[1].Desc);
                if (nodes.StartNode != null && nodes.EndNode != null)
                {
                    nodes.Search();
                }
            }
        }

        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            if (connecting)
            {
                e.Graphics.DrawLine(Pens.Black, actNode.X, actNode.Y, oldX, oldY);
            }
            nodes.Paint(e.Graphics);
        }

        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if(moving)
            {
                actNode.Move(e.X - oldX, e.Y - oldY);
            }
            oldX = e.X;
            oldY = e.Y;
            Invalidate();
        }

        private void miNew_Click(object sender, EventArgs e)
        {
            nodes = new NodeManagement();
            Invalidate();
        }

        private void miOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = "C:\\Temp";
            ofd.Filter = "Binary File | *.bin";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open);

                BinaryFormatter bf = new BinaryFormatter();

                nodes = (NodeManagement)bf.Deserialize(fs);

                nodes.Socket_Conn = socket_sender;
                nodes.send();

                Invalidate();

                fs.Close();
            }
        }

        private void miSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.InitialDirectory = "C:\\Temp";
            ofd.Filter = "Binary File | *.bin";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(ofd.FileName, FileMode.Create);

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, nodes);

                fs.Close();
            }
        }

        private void setStartNode_Click(object sender, EventArgs e)
        {
            nodes.ResetMarked();
            nodes.StartNode = actNode;
            Invalidate();
        }

        private void miSetEndNode_Click(object sender, EventArgs e)
        {
            nodes.ResetMarked();
            nodes.EndNode = actNode;
            
            nodes.Search();
            Invalidate();
        }

        private void miResetStartEndNode_Click(object sender, EventArgs e)
        {
            nodes.StartNode = null;
            nodes.EndNode = null;
            nodes.ResetMarked();
            Invalidate();
        }

        private void miConnect_Click(object sender, EventArgs e)
        {
            byte[] msg;

            IPEndPoint ipEndPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 1000);
            socket_sender = new Socket(SocketType.Stream, ProtocolType.Tcp);

            socket_sender.SendBufferSize = 8192;
            socket_sender.ReceiveBufferSize = 8192;

            socket_sender.Connect(ipEndPoint);

            diagNewClient diag = new diagNewClient();
            if (diag.ShowDialog() != DialogResult.OK)
                Close();

            string ClientName = diag.ClientName;

            msg = Encoding.ASCII.GetBytes(ClientName);
            socket_sender.Send(msg);

            miConnect.Enabled = false;
            miDisconnect.Enabled = true;

            Text = "Client " + ClientName + " (Connected)";

            nodes.Socket_Conn = socket_sender;
        }

        private void miDisconnect_Click(object sender, EventArgs e)
        {
            byte[] msg = Encoding.ASCII.GetBytes("bye");
            socket_sender.Send(msg);
            socket_sender.Close();
            socket_sender = null;

            Text = "Client (Disconnected)";

            miConnect.Enabled = true;
            miDisconnect.Enabled = false;
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] msg = Encoding.ASCII.GetBytes("print");
            socket_sender.Send(msg);
        }

        private void FrmMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (connecting)
            {
                Node endNode = nodes.IsInNode(e.X, e.Y);
                if(endNode != null && actNode != endNode)
                {
                    int dist = (int)actNode.Distance(endNode.X, endNode.Y);
                    //actNode.AddNeighbour(endNode, dist);
                    //endNode.AddNeighbour(actNode, dist);

                    nodes.AddConnection(actNode.Desc, endNode.Desc, dist);

                    Invalidate();
                }
            }
            connecting = false;
            moving = false;

        }
    }
}
