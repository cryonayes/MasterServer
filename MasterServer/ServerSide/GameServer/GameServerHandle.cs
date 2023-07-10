using System;
using MasterServer.Common;

namespace MasterServer.ServerSide.GameServer;

// GameServer tarafından gelen verileri işlemek için gereken fonksiyonlar
public static class GameServerHandle
{
    public static void Welcome(Packet _packet)
    {
        Console.WriteLine("Game Server bize cevap verdi woah");
    }
}