﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class frmMain : Form
    {
        private Socket socket_sender;

        public frmMain()
        {
            InitializeComponent();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void miConnect_Click(object sender, EventArgs e)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 1000);
            socket_sender = new Socket(SocketType.Stream, ProtocolType.Tcp);

            socket_sender.SendBufferSize = 8192;
            socket_sender.ReceiveBufferSize = 8192;

            socket_sender.Connect(ipEndPoint);
            Text = "Client (Connected)";
        }

        private void miDisconnect_Click(object sender, EventArgs e)
        {
            byte[] msg = Encoding.ASCII.GetBytes("bye");
            socket_sender.Send(msg);
            socket_sender.Close();
            socket_sender = null;

            Text = "Client (Disconnected)";
        }
    }
}
