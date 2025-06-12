using BluidingBlocks.CQRS;
using MediatR;
using Puja.Application.Commands;
using Puja.Application.DTOS;
using Puja.Domain.Repositories;
using Puja.Domain.ValueObjects;
using Puja.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using Puja.Domain.Aggregates;

namespace Puja.Application.Handlers
{
    public class CreatePujaCommandHandler : ICommandHandler<CreatePujaCommand, CreatePujaResult>
    {
        private readonly IPujaRepository _pujaRepository;
        private readonly IMediator _mediator;

        public CreatePujaCommandHandler(
            IPujaRepository pujaRepository,
            IMediator mediator)
        {
            _pujaRepository = pujaRepository ?? throw new ArgumentNullException(nameof(pujaRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<CreatePujaResult> Handle(CreatePujaCommand request, CancellationToken cancellationToken)
        {
            
            var ultimaPuja = await _pujaRepository.GetUltimaPujaPorSubastaIdAsync(request.Puja.SubastaId);

          
            var incrementoMinimo = await _pujaRepository.GetIncrementoMinimoPorSubastaIdAsync(request.Puja.SubastaId);

            decimal montoBase = ultimaPuja?.Monto.Value ?? await _pujaRepository.GetPrecioInicialPorSubastaIdAsync(request.Puja.SubastaId);

            
            if (request.Puja.Monto < montoBase + incrementoMinimo)
                throw new InvalidOperationException("El monto de la puja no respeta el incremento mínimo permitido.");

            // 4. Registrar la puja
            var pujaId = Guid.NewGuid().ToString();
            var puja = new AggregatePuja(
                new PujaId(pujaId),
                new SubastaId(request.Puja.SubastaId),
                new UserId(request.Puja.UserId),
                new Monto(request.Puja.Monto)
            );

            await _pujaRepository.AddAsync(puja);

            var pujaCreatedEvent = new PujaCreatedEvent(
                pujaId,
                request.Puja.SubastaId,
                request.Puja.UserId,
                request.Puja.Monto,
                request.Puja.FechaPuja
            );

            await _mediator.Publish(pujaCreatedEvent, cancellationToken);

            return new CreatePujaResult(Guid.Parse(pujaId));
        }
    }
}