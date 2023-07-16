using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MasterServer.Database.Helpers;

public class Player
{
    public ObjectId Id { get; set; }
    
    [BsonElement("username")]
    public string Username;

    [BsonElement("password")]
    public string Password;

    [BsonElement("score")]
    public int Score;
    
    [BsonElement("authToken")]
    public string Token;
}