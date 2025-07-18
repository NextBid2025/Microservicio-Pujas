using Puja.Application.Commands;
using Puja.Application.DTOS;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Puja.Domain.Repositories;
using Puja.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace Puja.API.Controllers
{
    [ApiController]
    [Route("api/puja")]
    public class PujaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPujaRepository _pujaRepository;
        private readonly IPujaService _pujaService;
        private readonly INotificacionPujaService _notificacionPujaService;
        private readonly IUsuarioService _usuarioService;
        private readonly ISubastaService _subastaService; // Servicio de subasta añadido

        public PujaController(
            IMediator mediator,
            IPujaRepository pujaRepository,
            IPujaService pujaService,
            INotificacionPujaService notificacionPujaService,
            IUsuarioService usuarioService,
            ISubastaService subastaService) // Inyección del servicio
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _pujaRepository = pujaRepository ?? throw new ArgumentNullException(nameof(pujaRepository));
            _pujaService = pujaService ?? throw new ArgumentNullException(nameof(pujaService));
            _notificacionPujaService = notificacionPujaService ?? throw new ArgumentNullException(nameof(notificacionPujaService));
            _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
            _subastaService = subastaService ?? throw new ArgumentNullException(nameof(subastaService)); // Asignación del servicio
        }

        /// <summary>
        /// Endpoint de prueba para verificar la conexión con el servicio de Usuarios.
        /// </summary>
        /// <param name="userId">El ID del usuario a verificar.</param>
        /// <returns>Un resultado indicando si la conexión y la validación fueron exitosas.</returns>
        [HttpGet("test-conexion-usuario/{userId}")]
        public async Task<IActionResult> TestConexionUsuario(string userId)
        {
            try
            {
                bool existe = await _usuarioService.UsuarioExisteAsync(userId);
                if (existe)
                {
                    return Ok($"Conexión exitosa: El usuario con ID '{userId}' existe.");
                }
                else
                {
                    return NotFound($"Conexión exitosa, pero el usuario con ID '{userId}' no fue encontrado.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fallo en la conexión con el servicio de usuarios: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea una nueva puja en una subasta activa.
        /// Valida que el usuario exista y que la subasta esté activa antes de crear la puja.
        /// Si la puja es válida, la notifica a todos los clientes conectados a la subasta.
        /// Si la puja es inválida (por ejemplo, monto insuficiente o reglas de negocio), notifica el error a los clientes y retorna un mensaje descriptivo.
        /// </summary>
        /// <param name="pujaDto">
        /// Objeto con los datos de la puja a crear: identificador de subasta, identificador de usuario y monto.
        /// </param>
        /// <returns>
        /// Un resultado HTTP 201 con la puja creada si es exitosa.
        /// Un resultado HTTP 400 con el motivo del error si el usuario no existe, la subasta no está activa o la puja es inválida.
        /// </returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreatePuja([FromBody] CreatePujaDto pujaDto)
        {
            var usuarioExiste = await _usuarioService.UsuarioExisteAsync(pujaDto.UserId);
            if (!usuarioExiste)
                return BadRequest("El usuario especificado no existe.");

            var subastaActiva = await _subastaService.SubastaEstaActivaAsync(pujaDto.SubastaId);
            if (!subastaActiva)
                return BadRequest("La subasta no está activa o no existe.");

            var puja = new PujaDto(
                Guid.NewGuid().ToString(),
                pujaDto.SubastaId,
                pujaDto.UserId,
                pujaDto.Monto,
                DateTime.UtcNow
            );

            try
            {
                var result = await _mediator.Send(new CreatePujaCommand(puja));
                await _notificacionPujaService.NotificarNuevaPujaAsync(puja.SubastaId, puja);
                return CreatedAtAction(nameof(CreatePuja), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                await _notificacionPujaService.NotificarPujaInvalidaAsync(puja.SubastaId, ex.Message);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("/Puja/health")]
        public IActionResult Health() => Ok("Healthy");

        /// <summary>
        /// Obtiene la puja ganadora (última puja realizada) de una subasta específica.
        /// </summary>
        /// <param name="subastaId">Identificador único de la subasta.</param>
        /// <returns>
        /// Un resultado HTTP 200 con la puja ganadora si existe.
        /// Un resultado HTTP 404 si no hay pujas para la subasta indicada.
        /// </returns>
        [HttpGet("ganadora/{subastaId}")]
        public async Task<IActionResult> GetPujaGanadora(string subastaId)
        {
            var puja = await _pujaRepository.GetUltimaPujaPorSubastaIdAsync(subastaId);
            if (puja == null)
                return NotFound("No hay pujas para esta subasta.");

            return Ok(puja);
        }

        /// <summary>
        /// Crea una puja automática para una subasta activa.
        /// Valida que la subasta esté activa antes de procesar la puja automática.
        /// </summary>
        /// <param name="dto">
        /// Objeto con los datos necesarios para la puja automática: identificador de usuario, identificador de subasta y monto.
        /// </param>
        /// <returns>
        /// Un resultado HTTP 200 con un mensaje de éxito si la puja automática se realiza correctamente.
        /// Un resultado HTTP 400 si la subasta no está activa o no existe.
        /// </returns>
        [HttpPost("puja-automatica")]
        public async Task<IActionResult> CrearPujaAutomatica([FromBody] PujaAutomaticaDto dto)
        {
            // Validar si la subasta está activa
            var subastaActiva = await _subastaService.SubastaEstaActivaAsync(dto.SubastaId);
            if (!subastaActiva)
            {
                return BadRequest("La subasta no está activa o no existe.");
            }

            await _pujaService.CrearPujaAutomaticaAsync(dto.UserId, dto.SubastaId, dto.Monto);
            return Ok(new { mensaje = "Puja automática realizada correctamente" });
        }

        /// <summary>
        /// Envía una notificación de prueba para verificar la conexión WebSocket.
        /// </summary>
        /// <param name="subastaId">El ID de la subasta para la notificación.</param>
        /// <param name="mensaje">El mensaje a enviar.</param>
        /// <returns>Un resultado de la acción.</returns>
        [HttpPost("notificacion")]
        public async Task<IActionResult> EnviarNotificacionPrueba(string subastaId = "subasta-prueba", string mensaje = "¡Nueva puja de prueba!")
        {
            var payload = new
            {
                SubastaId = subastaId,
                Mensaje = mensaje,
                Monto = 150.0m,
                UserId = "usuario-prueba"
            };

            await _notificacionPujaService.NotificarNuevaPujaAsync(subastaId, payload);

            return Ok($"Notificación de prueba enviada a la subasta '{subastaId}'.");
        }
    }
}