using MasterServer.Common;

namespace MasterServer.ServerSide.GameServer;

// GameServer'a veri göndermek için kullanılacak fonskiyonlar
public static class GameServerSend
{
    private static void SendTcpData(Packet _packet)
    {
        _packet.WriteLength();
        GameServerConn.Instance.SendData(_packet);
    }

    public static void SendLobbyInfo(string _lobbyId, int _capacity)
    {
        using var _packet = new Packet();
        _packet.Write(_lobbyId);
        _packet.Write(_capacity);

        SendTcpData(_packet);
    }
}