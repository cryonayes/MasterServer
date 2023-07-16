using MasterServer.Common;
using MasterServer.Database;
using MasterServer.Database.Helpers;
using MasterServer.ServerSide.Lobby;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MasterServer.ServerSide.MasterServer
{
    internal abstract class ClientHandle
    {
        public static void LoginReceived(int _fromClient, Packet _packet)
        {
            var _username = _packet.ReadString();
            var _password = _packet.ReadString();

            var _token = LoginHelper.HandleLogin(_username, _password);
            if (_token == "")
                ClientSend.LoginFailed(_fromClient);
            else
                ClientSend.LoginSuccess(_fromClient, _username, _token);
        }
        
        public static void RegisterReceived(int _fromClient, Packet _packet)
        {
            var _username = _packet.ReadString();
            var _password = _packet.ReadString();

            var _success = LoginHelper.HandleRegister(_username, _password);
            if (_success)
                ClientSend.RegisterSuccess(_fromClient);
            else
                ClientSend.RegisterFailed(_fromClient);
        }

        public static void LobbyRequestReceived(int _fromClient, Packet _packet)
        {
            var _lobby = LobbyManager.GetInstance().GetOrCreateLobby();
            _lobby.AddPlayer(_fromClient);
        }

        public static void OnPlayerFinished(int _fromclient, Packet _packet)
        {
            var _playerToken = _packet.ReadString();
            var _lobby = LobbyManager.GetInstance().GetLobyWithPlayerId(_fromclient);
            if (!_lobby.PlayerWon)
            {
                var _projection = Builders<BsonDocument>.Projection.Include("score").Exclude("_id");
                var _update = Builders<BsonDocument>.Update.Inc("score", 1);
                var _data = MongoCrud.Collection.FindOneAndUpdate(
                    filter: Builders<BsonDocument>.Filter.Eq("authToken", _playerToken),
                    update: _update,
                    options: new FindOneAndUpdateOptions<BsonDocument>
                    {
                        Projection = _projection,
                        ReturnDocument = ReturnDocument.After
                    }
                );
                if (_data != null)
                    _lobby.PlayerWon = true;
            }
            var _playersData = MongoCrud.Collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            ClientSend.ScoreTable(_fromclient, _playersData);
        }
    }
}
