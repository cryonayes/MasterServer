using System;
using System.Threading;
using MasterServer.Database;
using MasterServer.ServerSide.GameServer;
using MasterServer.ServerSide.MasterServer;
using MasterServer.Threading;

namespace MasterServer
{
    class Program
    {
        private static bool _isRunning;

        public static void Main()
        {
            while (true)
            {
                try
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
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Retrying connection...");
                }
            }
            
            _isRunning = true;
            MongoCrud.Connect(Globals.MongoUri, Globals.DatabaseName, Globals.CollectionName);
            
            var _mainThread = new Thread(MainThread);
            _mainThread.Start();

            Server.Start(1337);
            _mainThread.Join();
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
                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MsPerTick);

                    if (_nextLoop > DateTime.Now)
                        Thread.Sleep(_nextLoop - DateTime.Now);
                }
            }
        }
    }
}
