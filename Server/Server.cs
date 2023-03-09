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
            Socket clientHandler = (Socket)obj;
            byte[] bytes = new byte[8192];
            string clientName;
            string msg;

            Console.WriteLine(">> client connected");
            int bytesRec = clientHandler.Receive(bytes);
            clientName = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            Console.WriteLine(clientName);

            bytesRec = clientHandler.Receive(bytes);
            msg = Encoding.ASCII.GetString(bytes, 0, bytesRec);

            while (msg != "bye")
            {

            }
        }
    }
}
