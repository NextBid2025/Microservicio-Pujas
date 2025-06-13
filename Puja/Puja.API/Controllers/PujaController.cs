using Puja.Application.Commands;
using Puja.Application.DTOS;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Puja.API.Controllers
{
    [ApiController]
    [Route("api/pujas")]
    public class PujaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PujaController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePuja([FromBody] CreatePujaDto pujaDto)
        {
            var puja = new PujaDto(
                Guid.NewGuid().ToString(),
                pujaDto.SubastaId,
                pujaDto.UserId,
                pujaDto.Monto,
                DateTime.UtcNow
            );

            var result = await _mediator.Send(new CreatePujaCommand(puja));
            return CreatedAtAction(nameof(CreatePuja), new { id = result.Id });
        }
        [HttpGet("health")]
        public IActionResult Health() => Ok("Healthy");
    }
    
}