using System;
using System.Threading;
using MasterServer.Database;
using MasterServer.ServerSide.GameServer;
using MasterServer.ServerSide.Lobby;
using MasterServer.ServerSide.MasterServer;
using MasterServer.Threading;

namespace MasterServer
{
    class Program
    {
        private static bool _isRunning;

        public static void Main()
        {
            GameServerConn.CreateInstance(Globals.GameServerIp, Globals.GameServerPort);
            
            GameServerConn.Instance.OnConnected += (_sender, _args) =>
            {
                Console.WriteLine("Connected to the Game Server!");
            };
            
            GameServerConn.Instance.OnDisconnected += (_sender, _args) =>
            {
                Console.WriteLine("Disconnected from the Game Server!");
            };
            
            GameServerConn.Instance.Start();


            _isRunning = true;
            MongoCrud.Connect(Globals.MongoUri, Globals.DatabaseName, Globals.CollectionName);
            
            var _mainThread = new Thread(MainThread);
            _mainThread.Start();

            Server.Start(1337);
            LobbyManager.GameServerNotifyLobby += GameServerNotifyLobby;
            LobbyManager.LobbyIsFulled += NotifyLobbyPlayers;
        }

        static void GameServerNotifyLobby(object _sender, LobbyManager.Lobby _lobby)
        {
            // Lobby'nin kaç kişilik olduğu ve ID'si haber edilecek
            GameServerSend.SendLobbyInfo(_lobby.GetId(), _lobby.Capacity);
            Console.WriteLine($"Yeni lobby oluşturuldu, GameServer'a haber veriliyor ID: {_lobby.GetId()}");
        }

        static void NotifyLobbyPlayers(object _sender, LobbyManager.Lobby _lobby)
        {
            foreach (var _client in _lobby.GetPlayers())
                ServerSend.GoJoinLobby(_client);
        }
        
        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TicksPerSec} ticks per second.");
            var _nextLoop = DateTime.Now;

            while (_isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    ThreadManager.UpdateMain();
                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MsPerTick); // Calculate at what point in time the next tick should be executed

                    if (_nextLoop > DateTime.Now)
                        Thread.Sleep(_nextLoop - DateTime.Now); // Let the thread sleep until it's needed again.
                }
            }
        }
    }
}
