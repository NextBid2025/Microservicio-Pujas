using MongoDB.Bson;
using MongoDB.Driver;
using Pruja.Infrastructure.Configurations;
using Puja.Infraestructura.Interfaces;
using Puja.Infraestructura.Persistence.Repository.MongoRead.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puja.Infraestructura.Persistence.Repository.MongoRead
{
    public class MongoReadPujaRepository : IPujaReadRepository
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoReadPujaRepository(MongoReadDbConfig mongoConfig)
        {
            _collection = mongoConfig.Db.GetCollection<BsonDocument>("puja_read");
        }

        public async Task AddAsync(BsonDocument puja)
        {
            var id = puja.GetValue("_id", "").ToString(); // Forzar string
            puja["_id"] = id;
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, puja, new ReplaceOptions { IsUpsert = true });
        }
        public async Task<PujaMongoRead?> GetByIdAsync(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var doc = await _collection.Find(filter).FirstOrDefaultAsync();
            return doc == null ? null : PujaMongoRead.FromBson(doc);
        }

        public async Task<IEnumerable<PujaMongoRead>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("userId", userId);
            var docs = await _collection.Find(filter).ToListAsync();
            var result = new List<PujaMongoRead>();
            foreach (var doc in docs)
            {
                var mapped = PujaMongoRead.FromBson(doc);
                if (mapped != null)
                    result.Add(mapped);
            }
            return result;
        }

        public async Task<BsonDocument> GetUltimaPujaPorSubastaIdAsync(string subastaId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("subastaId", subastaId);
            var sort = Builders<BsonDocument>.Sort.Descending("monto");
            return await _collection.Find(filter).Sort(sort).FirstOrDefaultAsync();
        }
    }
}