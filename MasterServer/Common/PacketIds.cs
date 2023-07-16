namespace MasterServer.Common;

// Sent from master server to client
public enum MasterToClient
{
    Welcome = 1,
    LoginSuccess,
    LoginFailed,
    RegisterSuccess,
    RegisterFailed,
    GoJoinLobby,
    ScoreTable
}

public enum ClientToMaster
{
    Login = 10,
    Register,
    LobbyRequest,
    OnFinishLine
}

public enum GameServerToClient
{
    Welcome = 20,
}

public enum GameServerToMaster
{
    Welcome = 40,
    LobbyReady,
    PlayerConnected,
    PlayerDisconnected
}

public enum MasterToGameServer
{
    LobbyInfo = 50
}