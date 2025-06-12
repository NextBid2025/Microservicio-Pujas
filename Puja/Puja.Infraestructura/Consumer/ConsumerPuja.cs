using MassTransit;
using MongoDB.Bson;
using Puja.Domain.Events;
using Puja.Infraestructura.Interfaces;

namespace Puja.Infraestructura.Consumer;

public class ConsumerPuja(IPujaReadRepository pujaReadRepository) : IConsumer<PujaCreatedEvent>
{
    private readonly IPujaReadRepository _pujaReadRepository = pujaReadRepository;

    public async Task Consume(ConsumeContext<PujaCreatedEvent> @event)
    {
        var message = @event.Message;
        Console.WriteLine($"Mensaje recibido: {message}");

        var readDocument = new BsonDocument
        {
            { "_id", message.PujaId },
            { "subastaId", message.SubastaId },
            { "userId", message.UserId },
            { "monto", message.Monto },
            { "fechaPuja", message.FechaCreacion } // Usa el nombre correcto aquí
        };

        await _pujaReadRepository.AddAsync(readDocument);
        Console.WriteLine("Documento insertado en la base de datos de lectura.");
    }
}
