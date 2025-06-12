using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Pruja.Infrastructure.Configurations;
using Puja.Infraestructura.Interfaces;
using Puja.Infraestructura.Persistence.Repository.MongoRead.Documents;

namespace Puja.Infraestructura.Persistence.Repository.MongoRead;

public class MongoReadPujaRepository : IPujaReadRepository
{
    private readonly IMongoCollection<BsonDocument> _pujasCollection;

    public MongoReadPujaRepository(MongoReadDbConfig mongoConfig)
    {
        _pujasCollection = mongoConfig.Db.GetCollection<BsonDocument>("puja_read");
    }

    public async Task<PujaMongoRead?> GetByIdAsync(string id)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
        var bsonPuja = await _pujasCollection.Find(filter).FirstOrDefaultAsync();

        if (bsonPuja == null)
            return null;

        return new PujaMongoRead
        {
            Id = bsonPuja["_id"].AsString,
            SubastaId = bsonPuja["subastaId"].AsString,
            UserId = bsonPuja["userId"].AsString,
            Monto = bsonPuja["monto"].AsDecimal,
            FechaPuja = bsonPuja["fechaPuja"].ToUniversalTime()
        };
    }

    public async Task<IEnumerable<PujaMongoRead>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("userId", userId);
        var pujas = await _pujasCollection.Find(filter).ToListAsync();

        return pujas.Select(p => new PujaMongoRead
        {
            Id = p["_id"].AsString,
            SubastaId = p["subastaId"].AsString,
            UserId = p["userId"].AsString,
            Monto = p["monto"].AsDecimal,
            FechaPuja = p["fechaPuja"].ToUniversalTime()
        });
    }

    public async Task AddAsync(BsonDocument puja)
    {
        await _pujasCollection.InsertOneAsync(puja);
    }

    public async Task UpdateAsync(BsonDocument puja)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", puja["_id"]);
        await _pujasCollection.ReplaceOneAsync(filter, puja);
    }
}
    
    