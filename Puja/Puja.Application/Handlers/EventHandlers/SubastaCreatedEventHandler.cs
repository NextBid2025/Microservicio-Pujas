using MediatR;
using MassTransit;
using Puja.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Puja.Application.Handlers.EventHandlers
{
    public class PujaCreatedEventHandler : INotificationHandler<PujaCreatedEvent>
    {
        private readonly ISendEndpointProvider _publishEndpoint;

        public PujaCreatedEventHandler(ISendEndpointProvider publishEndpoint)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Handle(PujaCreatedEvent pujaCreatedEvent, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Puja creada: {pujaCreatedEvent.PujaId} - Monto: {pujaCreatedEvent.Monto}");
            // Enviar el evento a la cola de RabbitMQ
            await _publishEndpoint.Send(pujaCreatedEvent);
        }
    }
}