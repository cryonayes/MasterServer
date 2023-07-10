using System;
using MasterServer.Common;
using MasterServer.ServerSide.Lobby;

namespace MasterServer.ServerSide.GameServer;

// GameServer tarafından gelen verileri işlemek için gereken fonksiyonlar
public static class GameServerHandle
{
    public static void Welcome(Packet _packet)
    {
        Console.WriteLine("Game Server bize cevap verdi woah");
    }

    public static void LobbyIsReady(Packet _packet)
    {
        var _ready = _packet.ReadString();
        var _lobby = LobbyManager.GetInstance().GetLobbyById(_ready);
    }
}