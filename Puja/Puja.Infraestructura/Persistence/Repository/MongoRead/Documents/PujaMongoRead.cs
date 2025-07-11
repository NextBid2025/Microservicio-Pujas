using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Puja.Infraestructura.Persistence.Repository.MongoRead.Documents;

public class PujaMongoRead
{
    [BsonId]
    [BsonElement("_id")]
    public required string Id { get; set; }

    [BsonElement("subastaId")]
    public required string SubastaId { get; set; }

    [BsonElement("userId")]
    public required string UserId { get; set; }

    [BsonElement("monto")]
    public required decimal Monto { get; set; }

    [BsonElement("fechaPuja")]
    public required DateTime FechaPuja { get; set; }

    // Método de mapeo manual desde BsonDocument
    public static PujaMongoRead? FromBson(BsonDocument doc)
    {
        if (doc == null) return null;
        return new PujaMongoRead
        {
            Id = doc.GetValue("_id", "").AsString,
            SubastaId = doc.GetValue("subastaId", "").AsString,
            UserId = doc.GetValue("userId", "").AsString,
            Monto = doc.GetValue("monto", 0).ToDecimal(),
            FechaPuja = doc.GetValue("fechaPuja", BsonNull.Value).ToUniversalTime()
        };
    }
}