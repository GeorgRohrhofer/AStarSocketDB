using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.Http.Headers;

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

                    byte[] msgByte = Encoding.UTF8.GetBytes("ACK");
                    clientHandler.Send(msgByte);
                }
                else if (msg.StartsWith("addConnection"))
                {
                    line = msg.Substring(14, msg.Length - 15);
                    parts = line.Split(new char[] { ',' });
                    nodes.AddConnection(parts[0], parts[1], int.Parse(parts[2]));

                    byte[] msgByte = Encoding.UTF8.GetBytes("ACK");
                    clientHandler.Send(msgByte);
                }
                else if (msg.StartsWith("search"))
                {
                    line = msg.Substring(7, msg.Length - 8);
                    parts = line.Split(new char[] { ',' });
                    List<string> result = nodes.Search(parts[0], parts[1]);

                    string returnValue = "";

                    foreach(string s in result)
                    {
                        returnValue += s + ",";
                    }

                    returnValue = returnValue.Remove(returnValue.Length - 1); //Die Zeile ist ein Hurensohn.
                    byte[] msgByte = Encoding.UTF8.GetBytes(returnValue);
                    clientHandler.Send(msgByte);
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
                else if (msg.StartsWith("moveNode"))
                {
                    line = msg.Substring(9, msg.Length - 10);
                    parts = line.Split(new char[] { ',' });
                    nodes.moveNode(parts[0], int.Parse(parts[1]), int.Parse(parts[2]));
                }
                else
                {
                    Console.WriteLine("command not supported");
                }
            }
        }
    }
}
