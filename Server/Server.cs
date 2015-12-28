using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;


namespace Server
{
    class Server
    {
        static Socket listenerSocket;
        static List<ClientData> _client;


        static void Main(string[] args)
        {
            Console.WriteLine("staring server on " + Packet.GetIP4Address());
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _client = new List<ClientData>();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(Packet.GetIP4Address()),4242);
            listenerSocket.Bind(ip);
            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
        }

        public static void Data_IN(object cSocket) {
            Socket clientSocket = (Socket)cSocket;
            byte[] Buffer;
            int readBytes;
            for (; ;)
            {
                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(Buffer);
                    if (readBytes > 0)
                    {
                        Packet packet = new Packet(Buffer);
                        DataManager(packet);
                    }
                }
                catch (SocketException ex) {
                    //Console.WriteLine(ex);
                    Console.WriteLine("A Client Disconnected!");
                }
            }

        }

        public static void DataManager(Packet p) {
            switch (p.PacketType)
            {
                case PacketType.Registration:
                    break;
                case PacketType.Chat:
                    foreach (ClientData c in _client)
                    {
                        c.clientSocket.Send(p.ToBytes());
                    }
                    break;
                default:
                    break;
            }
            
        }
        static void ListenThread()
        {

            for (;;)
            {


                listenerSocket.Listen(0);
                _client.Add(new ClientData(listenerSocket.Accept()));
            }
        }

    }
    class ClientData
    {
        public Socket clientSocket;
        public Thread clientThread;
  
        public string id;

        public ClientData()
        {
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }

        public ClientData(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IN);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();

        }

        public void SendRegistrationPacket() {
            Packet p = new Packet(PacketType.Registration, "server");
            p.Gdata.Add(id);
            clientSocket.Send(p.ToBytes());
        }
    }
}
