using System.Collections.Generic;
using MasterServer.Common;
using MongoDB.Bson;

namespace MasterServer.ServerSide.MasterServer
{
    internal abstract class ClientSend
    {
        private static void SendTcpData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.Clients[_toClient].Conn.SendData(_packet);
        }

        #region Packets
        public static void Welcome(int _toClient, string _message)
        {
            using var _packet = new Packet((int)MasterToClient.Welcome);
            _packet.Write(_message);
            SendTcpData(_toClient, _packet);
        }
        
        public static void LoginFailed(int _toClient)
        {
            using var _packet = new Packet((int)MasterToClient.LoginFailed);
            SendTcpData(_toClient, _packet);
        }
        public static void LoginSuccess(int _toClient, string _username, string _token)
        {
            using var _packet = new Packet((int)MasterToClient.LoginSuccess);
            _packet.Write(_username);
            _packet.Write(_token);
            _packet.Write(Globals.GameServerIp);
            _packet.Write(Globals.GameServerPort);
            
            SendTcpData(_toClient, _packet);
        }

        public static void RegisterSuccess(int _toClient)
        {
            using var _packet = new Packet((int)MasterToClient.RegisterSuccess);
            SendTcpData(_toClient, _packet);
        }

        public static void RegisterFailed(int _toClient)
        {
            using var _packet = new Packet((int)MasterToClient.RegisterFailed);
            SendTcpData(_toClient, _packet);
        }

        public static void JoinToLobby(int _toClient, string _lobbyId)
        {
            using var _packet = new Packet((int)MasterToClient.GoJoinLobby);
            _packet.Write(_lobbyId);
            
            SendTcpData(_toClient, _packet);
        }

        #endregion

        public static void ScoreTable(int _toClient, List<BsonDocument> _playersData)
        {
            using var _packet = new Packet((int)MasterToClient.ScoreTable);
            
            _packet.Write(_playersData.Count);
            foreach (var _player in _playersData)
            {
                _packet.Write(_player.GetValue("username").AsString);
                _packet.Write(_player.GetValue("score").AsInt32);
            }
            SendTcpData(_toClient, _packet);
        }
    }
}
