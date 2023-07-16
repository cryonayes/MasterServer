using System;
using MasterServer.Common;
using MasterServer.Database.Helpers;

namespace MasterServer.ServerSide.GameServer;

public static class GameServerHandle
{
    public static void Welcome(Packet _packet)
    {
        var _myId = _packet.ReadInt();
        Console.WriteLine($"MasterServer Client ID: {_myId}");
    }

    public static void LobbyIsReady(Packet _packet)
    {
        var _ready = _packet.ReadString();
        Console.WriteLine($"Lobby ID {_ready} is ready.");
    }

    public static void PlayerConnected(Packet _packet)
    {
        var _playerId = _packet.ReadInt();
        Console.WriteLine($"Player {_playerId} is connected");
        Globals.GameServerPlayerCount += 1;
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        var _playerId = _packet.ReadInt();
        Console.WriteLine($"Player {_playerId} is disconnected");
        Globals.GameServerPlayerCount -= 1;
    }
}