using MongoDB.Bson;
using Puja.Infraestructura.Persistence.Repository.MongoRead.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puja.Infraestructura.Interfaces;

public interface IPujaReadRepository
{
    Task<PujaMongoRead?> GetByIdAsync(string id);
    Task<IEnumerable<PujaMongoRead>> GetByUserIdAsync(string userId);
    Task AddAsync(BsonDocument puja);
    
}