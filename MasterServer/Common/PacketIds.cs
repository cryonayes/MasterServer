namespace MasterServer.Common;

// Sent from server to client
public enum ServerPackets
{
    Welcome = 1,
    LoginSuccess,
    LoginFailed
}

// Sent from client to server.
public enum ClientPackets
{
    Login = 1,
    PlayerMovement
}