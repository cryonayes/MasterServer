using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MasterServer.ClientSide;
using MasterServer.Common;

namespace MasterServer.ServerSide.MasterServer
{
    static class Server
    {
        private static int Port { get; set; }
        public static readonly Dictionary<int, Client> Clients = new();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> PacketHandlers;

        private static TcpListener _tcpListener;

        public static void Start(int _port)
        {
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

            Console.WriteLine($"Server started on port {Port}.");
        }

        private static void TcpConnectCallback(IAsyncResult _result)
        {
            var _client = _tcpListener.EndAcceptTcpClient(_result);
            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            var _i = 1;
            while (true)
            {
                if (Clients.ContainsKey(_i))
                {
                    _i++;
                    continue;
                }
                Clients[_i] = new Client(_i);
                Clients[_i].Conn.Connect(_client);
                return;
            }
        }

        private static void InitializeServerData()
        {
            PacketHandlers = new Dictionary<int, PacketHandler>
            {
                { (int)ClientToMaster.Login, ClientHandle.LoginReceived },
                { (int)ClientToMaster.Register, ClientHandle.RegisterReceived },
                { (int)ClientToMaster.LobbyRequest, ClientHandle.LobbyRequestReceived },
                { (int)ClientToMaster.OnFinishLine, ClientHandle.OnPlayerFinished }
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}
