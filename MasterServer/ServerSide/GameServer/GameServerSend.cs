using MasterServer.Common;
using MasterServer.ServerSide.Lobby;

namespace MasterServer.ServerSide.GameServer;

public static class GameServerSend
{
    private static void SendTcpData(Packet _packet)
    {
        _packet.WriteLength();
        GameServerConn.Instance.SendData(_packet);
    }

    public static void GameServerNotifyLobby(LobbyManager.Lobby _lobby)
    {
        using var _packet = new Packet();
        
        _packet.Write(_lobby.GetId());
        _packet.Write(_lobby.Capacity);

        SendTcpData(_packet);
    }
}