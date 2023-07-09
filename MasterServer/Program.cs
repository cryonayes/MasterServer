using System;
using System.Threading;
using MasterServer.Database;
using MasterServer.ServerSide;
using MasterServer.Threading;

namespace MasterServer
{
    class Program
    {
        private static bool _isRunning;

        private static void Main()
        {
            _isRunning = true;

            MongoCrud.Connect(Globals.MongoUri, Globals.DatabaseName, Globals.CollectionName);
            
            var _mainThread = new Thread(MainThread);
            _mainThread.Start();

            Server.Start(1337);
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
