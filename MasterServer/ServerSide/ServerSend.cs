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
        public static void Welcome(int _toClient, string _message)
        {
            using var _packet = new Packet((int)ServerPackets.Welcome);
            _packet.Write(_message);
            SendTcpData(_toClient, _packet);
        }
        
        public static void LoginFailed(int _toClient)
        {
            using var _packet = new Packet((int)ServerPackets.LoginFailed);
            SendTcpData(_toClient, _packet);
        }
        public static void LoginSuccess(int _toClient, string _username, string _token)
        {
            using var _packet = new Packet((int)ServerPackets.LoginSuccess);
            _packet.Write(_username);
            _packet.Write(_token);
            _packet.Write(Globals.GameServerIp);
            _packet.Write(Globals.GameServerPort);
            
            SendTcpData(_toClient, _packet);
        }

        public static void WaitForLobby(int _toClient, string _lobbyId)
        {
            using var _packet = new Packet((int)ServerPackets.WaitingLobby);
            _packet.Write(_lobbyId);
            
            SendTcpData(_toClient, _packet);
        }

        public static void GoJoinLobby(int _toClient)
        {
            using var _packet = new Packet((int)ServerPackets.GoJoinLobby);
            SendTcpData(_toClient, _packet);
        }
        
        #endregion
    }
}
