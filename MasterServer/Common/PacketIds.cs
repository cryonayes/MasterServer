namespace MasterServer.Common;

// Sent from master server to client
public enum MasterToClient
{
    Welcome = 1,
    LoginSuccess,
    LoginFailed,
    WaitingLobby,
    GoJoinLobby
}

// Sent from client to master server.
public enum ClientToMaster
{
    Login = 1,
    JoinLobby
}

// Sent from game server to master server
public enum GameServerToMaster
{
    Welcome = 1
}

public enum MasterToGameServer
{
    LobbyInfo = 1
}