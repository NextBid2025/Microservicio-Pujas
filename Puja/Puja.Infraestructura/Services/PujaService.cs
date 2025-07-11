using Puja.Application.Interfaces;
using Puja.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Puja.Infraestructura.Services
{
    /// <summary>
    /// Servicio para gestionar la lógica de negocio de las pujas.
    /// </summary>
    public class PujaService : IPujaService
    {
        private readonly IPujaRepository _pujaRepository;
        private readonly ISubastaService _subastaService;
        private readonly INotificacionPujaService _notificacionPujaService;
        private readonly IUsuarioService _usuarioService;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PujaService"/>.
        /// </summary>
        /// <param name="pujaRepository">El repositorio para operaciones de escritura de pujas.</param>
        /// <param name="subastaService">El servicio para interactuar con el microservicio de subastas.</param>
        /// <param name="notificacionPujaService">El servicio para enviar notificaciones de pujas.</param>
        /// <param name="usuarioService">El servicio para interactuar con el microservicio de usuarios.</param>
        public PujaService(
            IPujaRepository pujaRepository,
            ISubastaService subastaService,
            INotificacionPujaService notificacionPujaService,
            IUsuarioService usuarioService)
        {
            _pujaRepository = pujaRepository;
            _subastaService = subastaService;
            _notificacionPujaService = notificacionPujaService;
            _usuarioService = usuarioService; // Falta asignar este parámetro al campo correspondiente.
        }

        /// <summary>
        /// Crea una puja automática para una subasta específica.
        /// </summary>
        /// <param name="userId">El identificador del usuario que realiza la puja.</param>
        /// <param name="subastaId">El identificador de la subasta.</param>
        /// <param name="monto">El monto de la puja.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si el usuario no es válido, la subasta no está activa o el monto no cumple con el incremento mínimo.</exception>
        public async Task CrearPujaAutomaticaAsync(string userId, string subastaId, decimal monto)
        {
            // Validar que el usuario exista
            if (!await _usuarioService.UsuarioExisteAsync(userId))
                throw new InvalidOperationException("El usuario no es válido.");
            // Validar que la subasta esté activa
            if (!await _subastaService.SubastaEstaActivaAsync(subastaId))
                throw new InvalidOperationException("La subasta no está activa.");

            // Validar que el monto sea válido (puedes agregar más reglas)
            var incrementoMinimo = await _pujaRepository.GetIncrementoMinimoPorSubastaIdAsync(subastaId);
            var ultimaPuja = await _pujaRepository.GetUltimaPujaPorSubastaIdAsync(subastaId);
            decimal montoBase = ultimaPuja?.Monto.Value ?? await _pujaRepository.GetPrecioInicialPorSubastaIdAsync(subastaId);

            if (monto < montoBase + incrementoMinimo)
                throw new InvalidOperationException("El monto de la puja automática no respeta el incremento mínimo.");

            // Crear la puja automática (ajusta según tu modelo de dominio)
            var pujaId = Guid.NewGuid().ToString();
            var puja = new Puja.Domain.Aggregates.AggregatePuja(
                new Puja.Domain.ValueObjects.PujaId(pujaId),
                new Puja.Domain.ValueObjects.SubastaId(subastaId),
                new Puja.Domain.ValueObjects.UserId(userId),
                new Puja.Domain.ValueObjects.Monto(monto)
            );

            await _pujaRepository.AddAsync(puja);

            // Actualizar el precio de la subasta
            await _subastaService.ActualizarPrecioSubastaAsync(subastaId, monto);

            // Notificar a todos los usuarios de la subasta sobre la nueva puja
            await _notificacionPujaService.NotificarNuevaPujaAsync(subastaId, new
            {
                UserId = userId,
                SubastaId = subastaId,
                Monto = monto
            });
        }
    }
}