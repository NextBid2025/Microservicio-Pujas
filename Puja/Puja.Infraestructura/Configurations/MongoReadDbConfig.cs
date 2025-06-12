using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Pruja.Infrastructure.Configurations;

public class MongoReadDbConfig
{
    private readonly MongoClient _client;
    public IMongoDatabase Db { get; }
    public MongoClient Client => _client; // Expose the client

    public MongoReadDbConfig()
    {
        string connectionUri = Environment.GetEnvironmentVariable("MONGODB_CNN_READ");
        string databaseName = Environment.GetEnvironmentVariable("MONGODB_NAME_READ");

        if (string.IsNullOrWhiteSpace(connectionUri) || string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException("La configuración de MongoDB para lectura no está definida.");

        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        _client = new MongoClient(settings);
        Db = _client.GetDatabase(Environment.GetEnvironmentVariable("MONGODB_NAME_READ"));
    }

    
}