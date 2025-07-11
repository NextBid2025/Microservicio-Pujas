using MongoDB.Bson.Serialization.Attributes;

namespace Puja.Infraestructura.Persistence.Repository.MongoWrite.Documents;

public class PujaAutomaticaConfigMongoWrite
{
    [BsonId]
    [BsonElement("_id")]
    public required string Id { get; set; }

    [BsonElement("subastaId")]
    public required string SubastaId { get; set; }

    [BsonElement("userId")]
    public required string UserId { get; set; }

    [BsonElement("montoMaximo")]
    public required decimal MontoMaximo { get; set; }

    [BsonElement("fechaCreacion")]
    public required DateTime FechaCreacion { get; set; }
}