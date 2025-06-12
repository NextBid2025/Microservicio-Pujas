using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Productos.Infrastructure.Configurations;
using Puja.Domain.Aggregates;
using Puja.Domain.Factories;
using Puja.Domain.Repositories;
using Puja.Domain.ValueObjects;

namespace Puja.Infraestructura.Persistence.Repository.MongoWrite
{
    public class MongoWritePujaRepository : IPujaRepository
    {
        private readonly IMongoCollection<BsonDocument> _pujasCollection;

        public MongoWritePujaRepository(MongoWriteDbConfig mongoConfig)
        {
            _pujasCollection = mongoConfig.Db.GetCollection<BsonDocument>("pujas_write");
        }

        public async Task AddAsync(AggregatePuja puja)
        {
            var bsonPuja = new BsonDocument
            {
                { "_id", puja.Id.Value },
                { "subastaId", puja.Id.Value },
                { "userId", puja.UserId.Value },
                { "monto", puja.Monto.Value },
                { "fechaPuja", puja.FechaPuja }
            };

            await _pujasCollection.InsertOneAsync(bsonPuja);
        }

        public async Task<AggregatePuja?> GetByIdAsync(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var bsonPuja = await _pujasCollection.Find(filter).FirstOrDefaultAsync();

            if (bsonPuja == null)
                return null;

            return PujaFactory.Create(
                new PujaId(bsonPuja["_id"].AsString),
                new SubastaId(bsonPuja["subastaId"].AsString),
                new UserId(bsonPuja["userId"].AsString),
                new Monto(bsonPuja["monto"].AsDecimal)
            );
        }

        // Implementación requerida por la interfaz (con CancellationToken)
        public async Task<AggregatePuja?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var bsonPuja = await _pujasCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

            if (bsonPuja == null)
                return null;

            return PujaFactory.Create(
                new PujaId(bsonPuja["_id"].AsString),
                new SubastaId(bsonPuja["subastaId"].AsString),
                new UserId(bsonPuja["userId"].AsString),
                new Monto(bsonPuja["monto"].AsDecimal)
            );
        }

        public async Task<IEnumerable<AggregatePuja>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("userId", userId);
            var pujas = await _pujasCollection.Find(filter).ToListAsync();

            return pujas.Select(p => PujaFactory.Create(
                new PujaId(p["_id"].AsString),
                new SubastaId(p["subastaId"].AsString),
                new UserId(p["userId"].AsString),
                new Monto(p["monto"].AsDecimal)
            ));
        }

        public async Task<decimal> GetIncrementoMinimoPorSubastaIdAsync(string subastaId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("subastaId", subastaId);
            var bsonPuja = await _pujasCollection.Find(filter).FirstOrDefaultAsync();
            return bsonPuja != null && bsonPuja.Contains("incrementoMinimo") ? bsonPuja["incrementoMinimo"].AsDecimal : 0;
        }

        public async Task<AggregatePuja> GetLastBidAsync(int subastaId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("subastaId", subastaId.ToString());
            var bsonPuja = await _pujasCollection.Find(filter)
                .Sort(Builders<BsonDocument>.Sort.Descending("fechaPuja"))
                .FirstOrDefaultAsync();

            if (bsonPuja == null)
                throw new KeyNotFoundException("No se encontró la última puja.");

            return PujaFactory.Create(
                new PujaId(bsonPuja["_id"].AsString),
                new SubastaId(bsonPuja["subastaId"].AsString),
                new UserId(bsonPuja["userId"].AsString),
                new Monto(bsonPuja["monto"].AsDecimal)
            );
        }

        public async Task<decimal> GetPrecioInicialPorSubastaIdAsync(string subastaId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("subastaId", subastaId);
            var bsonPuja = await _pujasCollection.Find(filter).FirstOrDefaultAsync();
            return bsonPuja != null && bsonPuja.Contains("precioInicial") ? bsonPuja["precioInicial"].AsDecimal : 0;
        }

        public async Task<AggregatePuja?> GetUltimaPujaPorSubastaIdAsync(string subastaId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("subastaId", subastaId);
            var bsonPuja = await _pujasCollection.Find(filter)
                .Sort(Builders<BsonDocument>.Sort.Descending("fechaPuja"))
                .FirstOrDefaultAsync();

            if (bsonPuja == null)
                return null;

            return PujaFactory.Create(
                new PujaId(bsonPuja["_id"].AsString),
                new SubastaId(bsonPuja["subastaId"].AsString),
                new UserId(bsonPuja["userId"].AsString),
                new Monto(bsonPuja["monto"].AsDecimal)
            );
        }
    }
}