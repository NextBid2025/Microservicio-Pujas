using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Productos.Infrastructure.Configurations;

public class MongoWriteDbConfig
{
    private readonly MongoClient _client;
    public IMongoDatabase Db { get; }
    public MongoClient Client => _client; // Expose the client

    public MongoWriteDbConfig()
    {
        string connectionUri = Environment.GetEnvironmentVariable("MONGODB_CNN_WRITE");
        string databaseName = Environment.GetEnvironmentVariable("MONGODB_NAME_WRITE");

        if (string.IsNullOrWhiteSpace(connectionUri) || string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException("La configuración de MongoDB para escritura no está definida.");

        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        settings.ConnectTimeout = TimeSpan.FromSeconds(60); // Incrementa el tiempo de espera

        _client = new MongoClient(settings);
        Db = _client.GetDatabase(Environment.GetEnvironmentVariable("MONGODB_NAME_WRITE"));
    }

    
}