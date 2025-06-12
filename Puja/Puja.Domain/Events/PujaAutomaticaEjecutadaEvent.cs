using MediatR;
using System;

namespace Puja.Domain.Events
{
    public class PujaAutomaticaEjecutadaEvent : INotification
    {
        public string PujaId { get; }
        public string SubastaId { get; }
        public string UserId { get; }
        public decimal Monto { get; }
        public DateTime FechaEjecucion { get; }

        public PujaAutomaticaEjecutadaEvent(
            string pujaId,
            string subastaId,
            string userId,
            decimal monto,
            DateTime fechaEjecucion)
        {
            PujaId = pujaId;
            SubastaId = subastaId;
            UserId = userId;
            Monto = monto;
            FechaEjecucion = fechaEjecucion;
        }
    }
}