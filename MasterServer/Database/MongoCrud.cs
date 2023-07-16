using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MasterServer.Database;

public static class MongoCrud
{
    public static IMongoCollection<BsonDocument> Collection;

    public static void Connect(string _connectionUri, string _databaseName, string _collectionName)
    {
        var _database = new MongoClient(_connectionUri).GetDatabase(_databaseName);
        Collection = _database.GetCollection<BsonDocument>(_collectionName);
    }

    public static void Create(BsonDocument _document)
    {
        Collection.InsertOne(_document);
    }

    public static List<BsonDocument> Read(FilterDefinition<BsonDocument> _filter)
    {
        return Collection.Find(_filter).ToList();
    }

    public static void Update(FilterDefinition<BsonDocument> _filter, UpdateDefinition<BsonDocument> update)
    {
        Collection.UpdateOne(_filter, update);
    }

    public static void Delete(FilterDefinition<BsonDocument> _filter)
    {
        Collection.DeleteOne(_filter);
    }
}