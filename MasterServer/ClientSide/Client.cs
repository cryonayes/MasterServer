using System;

namespace MasterServer.ClientSide
{
    class Client
    {
        public const int DataBufferSize = 4096;
        public readonly ClientConn Conn;

        public Client(int _clientId)
        {
            Conn = new ClientConn(_clientId);
        }
        
        public void Disconnect()
        {
            Console.WriteLine($"{Conn.Socket.Client.RemoteEndPoint} has disconnected.");
            Conn.Disconnect();
        }
    }
}
