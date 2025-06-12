using MediatR;
using System;

namespace Puja.Domain.Events
{
    public class PujaAutomaticaConfiguradaEvent : INotification
    {
        public string UserId { get; }
        public string SubastaId { get; }
        public decimal MontoMaximo { get; }
        public DateTime FechaConfiguracion { get; }

        public PujaAutomaticaConfiguradaEvent(
            string userId,
            string subastaId,
            decimal montoMaximo,
            DateTime fechaConfiguracion)
        {
            UserId = userId;
            SubastaId = subastaId;
            MontoMaximo = montoMaximo;
            FechaConfiguracion = fechaConfiguracion;
        }
    }
}