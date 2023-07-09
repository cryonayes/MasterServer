using MasterServer.Common;

namespace MasterServer.ServerSide
{
    internal abstract class ServerSend
    {
        private static void SendTcpData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.Clients[_toClient].Conn.SendData(_packet);
        }

        #region Packets
        public static void LoginFailed(int _toClient)
        {
            using var _packet = new Packet((int)ServerPackets.LoginFailed);
            
            SendTcpData(_toClient, _packet);
        }
        public static void LoginSuccess(int _toClient, string token)
        {
            using var _packet = new Packet((int)ServerPackets.LoginSuccess);
            _packet.Write(token);
            
            SendTcpData(_toClient, _packet);
        }
        
        #endregion
    }
}
