using MongoDB.Bson.Serialization.Attributes;

namespace Puja.Infraestructura.Persistence.Repository.MongoRead.Documents;

public class PujaMongoWrite
{
    [BsonId]
    [BsonElement("_id")]
    public required string Id { get; set; } // ID de la puja

    [BsonElement("subastaId")]
    public required string SubastaId { get; set; }

    [BsonElement("userId")]
    public required string UserId { get; set; }

    [BsonElement("monto")]
    public required decimal Monto { get; set; }

    [BsonElement("fechaPuja")]
    public required DateTime FechaPuja { get; set; }
}