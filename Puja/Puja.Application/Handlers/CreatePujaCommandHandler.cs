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
    public class CreatePujaCommandHandler : IRequestHandler<CreatePujaCommand, CreatePujaResult>
    {
        private readonly IPujaRepository _pujaRepository;
        private readonly IMediator _mediator;
        private readonly ISubastaService _subastaService;
        private readonly INotificacionPujaService _notificacionPujaService;
        private readonly IPujaAutomaticaRepository _pujaAutomaticaRepository;
        private readonly IPujaService _pujaService;

        public CreatePujaCommandHandler(
            IPujaRepository pujaRepository,
            IMediator mediator,
            ISubastaService subastaService,
            INotificacionPujaService notificacionPujaService,
            IPujaAutomaticaRepository pujaAutomaticaRepository,
            IPujaService pujaService
        )
        {
            _pujaRepository = pujaRepository ?? throw new ArgumentNullException(nameof(pujaRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _subastaService = subastaService ?? throw new ArgumentNullException(nameof(subastaService));
            _notificacionPujaService = notificacionPujaService ?? throw new ArgumentNullException(nameof(notificacionPujaService));
            _pujaAutomaticaRepository = pujaAutomaticaRepository ?? throw new ArgumentNullException(nameof(pujaAutomaticaRepository));
            _pujaService = pujaService ?? throw new ArgumentNullException(nameof(pujaService));
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

            await _subastaService.ActualizarPrecioSubastaAsync(request.Puja.SubastaId, request.Puja.Monto);

            await _notificacionPujaService.NotificarNuevaPujaAsync(request.Puja.SubastaId, new {
                PujaId = puja.Id,
                UserId = request.Puja.UserId,
                Monto = request.Puja.Monto,
                FechaPuja = request.Puja.FechaPuja
            });

            var pujaCreatedEvent = new PujaCreatedEvent(
                pujaId,
                request.Puja.SubastaId,
                request.Puja.UserId,
                request.Puja.Monto,
                request.Puja.FechaPuja
            );

            await _mediator.Publish(pujaCreatedEvent, cancellationToken);

            // --- Lógica de puja automática ---
            var configs = await _pujaAutomaticaRepository.GetBySubastaIdAsync(request.Puja.SubastaId);

            foreach (var config in configs)
            {
                // Asegúrate que config tiene las propiedades UserId y MontoMaximo
                if (config.UserId != request.Puja.UserId && request.Puja.Monto < config.MontoMaximo)
                {
                    var nuevoMonto = Math.Min(config.MontoMaximo, request.Puja.Monto + incrementoMinimo);
                    await _pujaService.CrearPujaAutomaticaAsync(config.UserId, request.Puja.SubastaId, nuevoMonto);
                    await _notificacionPujaService.NotificarPujaAutomaticaAsync(config.UserId, request.Puja.SubastaId, nuevoMonto);
                }
                else if (request.Puja.Monto >= config.MontoMaximo)
                {
                    await _notificacionPujaService.NotificarLimiteAlcanzadoAsync(config.UserId, request.Puja.SubastaId);
                }
            }
            // --- Fin lógica de puja automática ---

            return new CreatePujaResult(Guid.Parse(pujaId));
        }
    }
}