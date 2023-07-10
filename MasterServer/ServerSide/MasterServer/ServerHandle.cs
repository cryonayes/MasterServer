using MasterServer.Common;
using MasterServer.Database.Helpers;
using MasterServer.ServerSide.Lobby;

namespace MasterServer.ServerSide.MasterServer
{
    internal abstract class ServerHandle
    {
        public static void LoginReceived(int _fromClient, Packet _packet)
        {
            var _username = _packet.ReadString();
            var _password = _packet.ReadString();

            var _token = LoginHelper.HandleLogin(_username, _password);
            if (_token == "")
                ServerSend.LoginFailed(_fromClient);
            else
                ServerSend.LoginSuccess(_fromClient, _username, _token);
        }

        public static void LobbyRequestReceived(int _fromClient, Packet _packet)
        {
            var _lobby = LobbyManager.GetInstance().GetOrCreateLobby();
            _lobby.AddPlayer(_fromClient);
            ServerSend.WaitForLobby(_fromClient, _lobby.GetId());
        }
    }
}
