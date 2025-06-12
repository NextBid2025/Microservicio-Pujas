using MediatR;
using System;

namespace Puja.Domain.Events
{
    public class PujaRechazadaEvent : INotification
    {
        public string PujaId { get; }
        public string SubastaId { get; }
        public string UserId { get; }
        public decimal Monto { get; }
        public string Motivo { get; }
        public DateTime FechaRechazo { get; }

        public PujaRechazadaEvent(
            string pujaId,
            string subastaId,
            string userId,
            decimal monto,
            string motivo,
            DateTime fechaRechazo)
        {
            PujaId = pujaId;
            SubastaId = subastaId;
            UserId = userId;
            Monto = monto;
            Motivo = motivo;
            FechaRechazo = fechaRechazo;
        }
    }
}