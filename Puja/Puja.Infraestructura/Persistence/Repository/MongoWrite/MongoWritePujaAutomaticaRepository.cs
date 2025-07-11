using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Puja.Domain.Entities;
using Puja.Domain.Repositories;
using Productos.Infrastructure.Configurations;
using Puja.Infraestructura.Persistence.Repository.MongoWrite.Documents;

namespace Puja.Infraestructura.Persistence.Repository.MongoWrite
{
    public class MongoWritePujaAutomaticaRepository : IPujaAutomaticaRepository
    {
        private readonly IMongoCollection<BsonDocument> _pujasAutomaticaCollection;

        public MongoWritePujaAutomaticaRepository(MongoWriteDbConfig mongoConfig)
        {
            _pujasAutomaticaCollection = mongoConfig.Db.GetCollection<BsonDocument>("pujas_automatica_write");
        }

        public Task<IEnumerable<PujaAutomaticaConfig>> ObtenerPorSubastaIdAsync(string subastaId)
        {
            // Implementación real aquí
            throw new System.NotImplementedException();
        }

        public Task<PujaAutomaticaConfig> ObtenerPorUsuarioYSubastaAsync(string userId, string subastaId)
        {
            // Implementación real aquí
            throw new System.NotImplementedException();
        }

        public Task CrearAsync(PujaAutomaticaConfig pujaAutomatica)
        {
            // Implementación real aquí
            throw new System.NotImplementedException();
        }

        public Task ActualizarAsync(PujaAutomaticaConfig pujaAutomatica)
        {
            // Implementación real aquí
            throw new System.NotImplementedException();
        }

        public Task EliminarAsync(string id)
        {
            // Implementación real aquí
            throw new System.NotImplementedException();
        }
        
        public async Task<IEnumerable<PujaAutomaticaConfig>> GetBySubastaIdAsync(string subastaId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("subastaId", subastaId);
            var documentos = await _pujasAutomaticaCollection.Find(filter).ToListAsync();

            var resultado = new List<PujaAutomaticaConfig>();
            foreach (var doc in documentos)
            {
                var mongoWrite = BsonSerializer.Deserialize<PujaAutomaticaConfigMongoWrite>(doc);
                resultado.Add(new PujaAutomaticaConfig
                {
                    Id = mongoWrite.Id,
                    SubastaId = mongoWrite.SubastaId,
                    UserId = mongoWrite.UserId,
                    MontoMaximo = mongoWrite.MontoMaximo,
                    FechaCreacion = mongoWrite.FechaCreacion
                });
            }
            return resultado;
        }
}
}