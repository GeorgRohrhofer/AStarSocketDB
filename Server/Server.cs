using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server
{
    internal class Server
    {
        public void Start()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 1000);
            Socket listener = new Socket(SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(ipEndPoint);
            listener.Listen(100);

            Socket clientHandler;

            Console.WriteLine(">> server started");

            while (true)
            {
                clientHandler = listener.Accept();
                (new Thread(new ParameterizedThreadStart(ClientHandler))).Start(clientHandler);
            }
        }

        public void ClientHandler(object obj)
        {
            NodeManagement nodes = new NodeManagement();
            Socket clientHandler = (Socket)obj;
            byte[] bytes = new byte[8192];
            string msg="";
            string client;
            

            int bytesRec = clientHandler.Receive(bytes);
            client = Encoding.ASCII.GetString(bytes, 0, bytesRec);

            Console.WriteLine(">> client " + client + " connected");

            string line;
            string[] parts;
            while (msg != "bye")
            {
                bytesRec = clientHandler.Receive(bytes);
                msg = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                Console.WriteLine(msg);

                if (msg.StartsWith("addPoint"))
                {
                    line = msg.Substring(9, msg.Length - 10);
                    parts = line.Split(new char[] { ',' });
                    nodes.Add(parts[0],  int.Parse(parts[1]), int.Parse(parts[2]));
                }
                else if (msg.StartsWith("addConnection"))
                {
                    line = msg.Substring(14, msg.Length - 15);
                    parts = line.Split(new char[] { ',' });
                    nodes.AddConnection(parts[0], parts[1], int.Parse(parts[2]));
                }
                else if (msg.StartsWith("search"))
                {
                    line = msg.Substring(7, msg.Length - 8);
                    parts = line.Split(new char[] { ',' });
                    nodes.Search(parts[0], parts[1]);
                }
                else if (msg.StartsWith("print"))
                {
                    nodes.Print();
                }
                else if (msg.StartsWith("reset"))
                {
                    Console.WriteLine("reset");
                }
                else if (msg.StartsWith("bye"))
                {
                    Console.WriteLine("bye");
                }
                else
                {
                    Console.WriteLine("command not supported");
                }
            }
        }
    }
}
