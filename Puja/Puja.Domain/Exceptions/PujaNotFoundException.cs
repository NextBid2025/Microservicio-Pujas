namespace Puja.Domain.Exceptions
{
    /// <summary>
    /// Excepción que se lanza cuando no se encuentra una puja en el sistema.
    /// </summary>
    public class PujaNotFoundException : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PujaNotFoundException"/> con un mensaje de error específico.
        /// </summary>
        /// <param name="message">Mensaje que describe el error.</param>
        public PujaNotFoundException(string message) : base(message)
        {
        }
    }
}