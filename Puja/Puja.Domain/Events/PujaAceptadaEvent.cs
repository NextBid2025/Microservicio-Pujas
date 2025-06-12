using MediatR;
using System;

namespace Puja.Domain.Events
{
    public class PujaAceptadaEvent : INotification
    {
        public string PujaId { get; }
        public string SubastaId { get; }
        public string UserId { get; }
        public decimal Monto { get; }
        public DateTime FechaAceptada { get; }

        public PujaAceptadaEvent(
            string pujaId,
            string subastaId,
            string userId,
            decimal monto,
            DateTime fechaAceptada)
        {
            PujaId = pujaId;
            SubastaId = subastaId;
            UserId = userId;
            Monto = monto;
            FechaAceptada = fechaAceptada;
        }
    }
}