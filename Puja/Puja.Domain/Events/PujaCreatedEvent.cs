using MediatR;
using System;

namespace Puja.Domain.Events
{
    public class PujaCreatedEvent : INotification
    {
        public string PujaId { get; }
        public string SubastaId { get; }
        public string UserId { get; }
        public decimal Monto { get; }
        public DateTime FechaCreacion { get; }

        public PujaCreatedEvent(
            string pujaId,
            string subastaId,
            string userId,
            decimal monto,
            DateTime fechaCreacion)
        {
            PujaId = pujaId;
            SubastaId = subastaId;
            UserId = userId;
            Monto = monto;
            FechaCreacion = fechaCreacion;
        }
    }
}