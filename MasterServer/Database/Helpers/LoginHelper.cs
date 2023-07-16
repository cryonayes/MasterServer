using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MasterServer.Database.Helpers;

public static class LoginHelper
{
    public static string HandleLogin(string _username, string _password)
    {
        var _filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("username", _username),
            Builders<BsonDocument>.Filter.Eq("password", _password)
        );
        var _read = MongoCrud.Read(_filter);
        return _read.Count == 0 ? "" : UpdateAuthToken(_filter);
    }
    
    public static bool HandleRegister(string _username, string _password)
    {
        var _filter = Builders<BsonDocument>.Filter.Eq("username", _username);
        var _count = MongoCrud.Collection.CountDocuments(_filter);
        if (_count > 0) return false;
        var _newPlayer = new Player
        {
            Username = _username,
            Password = _password,
            Score = 0,
        };
        MongoCrud.Create(_newPlayer.ToBsonDocument());
        return true;
    }

    private static string UpdateAuthToken(FilterDefinition<BsonDocument> _filter)
    {
        var _authToken = Guid.NewGuid().ToString();
        MongoCrud.Collection.UpdateOne(_filter, Builders<BsonDocument>.Update.Set("authToken", _authToken));
        return _authToken;
    }
}