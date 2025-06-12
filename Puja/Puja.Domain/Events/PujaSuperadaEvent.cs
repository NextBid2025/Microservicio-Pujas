using MediatR;
using System;

namespace Puja.Domain.Events
{
    public class PujaSuperadaEvent : INotification
    {
        public string PujaIdAnterior { get; }
        public string SubastaId { get; }
        public string UserIdAnterior { get; }
        public decimal MontoAnterior { get; }
        public string PujaIdNueva { get; }
        public string UserIdNuevo { get; }
        public decimal MontoNuevo { get; }
        public DateTime FechaSuperada { get; }

        public PujaSuperadaEvent(
            string pujaIdAnterior,
            string subastaId,
            string userIdAnterior,
            decimal montoAnterior,
            string pujaIdNueva,
            string userIdNuevo,
            decimal montoNuevo,
            DateTime fechaSuperada)
        {
            PujaIdAnterior = pujaIdAnterior;
            SubastaId = subastaId;
            UserIdAnterior = userIdAnterior;
            MontoAnterior = montoAnterior;
            PujaIdNueva = pujaIdNueva;
            UserIdNuevo = userIdNuevo;
            MontoNuevo = montoNuevo;
            FechaSuperada = fechaSuperada;
        }
    }
}