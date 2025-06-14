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
using Puja.Application.Interfaces;

namespace Puja.Application.Handlers
{
    public class CreatePujaCommandHandler : ICommandHandler<CreatePujaCommand, CreatePujaResult>
    {
        private readonly IPujaRepository _pujaRepository;
        private readonly IMediator _mediator;
        private readonly ISubastaService _subastaService;

        public CreatePujaCommandHandler(
            IPujaRepository pujaRepository,
            IMediator mediator,
            ISubastaService subastaService)
        {
            _pujaRepository = pujaRepository ?? throw new ArgumentNullException(nameof(pujaRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _subastaService = subastaService ?? throw new ArgumentNullException(nameof(subastaService));
        }

        public async Task<CreatePujaResult> Handle(CreatePujaCommand request, CancellationToken cancellationToken)
        {
            bool subastaActiva;
            try
            {
                subastaActiva = await _subastaService.SubastaEstaActivaAsync(request.Puja.SubastaId);
            }
            catch (Exception ex)
            {
                // Si no se puede validar, no permitir la puja
                throw new InvalidOperationException("No se pudo validar el estado de la subasta.", ex);
            }

            if (!subastaActiva)
                throw new InvalidOperationException("La subasta no está activa.");

            var ultimaPuja = await _pujaRepository.GetUltimaPujaPorSubastaIdAsync(request.Puja.SubastaId);
            var incrementoMinimo = await _pujaRepository.GetIncrementoMinimoPorSubastaIdAsync(request.Puja.SubastaId);

            decimal montoBase = ultimaPuja?.Monto.Value ??
                                await _pujaRepository.GetPrecioInicialPorSubastaIdAsync(request.Puja.SubastaId);

            if (request.Puja.Monto < montoBase + incrementoMinimo)
                throw new InvalidOperationException("El monto de la puja no respeta el incremento mínimo permitido.");

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