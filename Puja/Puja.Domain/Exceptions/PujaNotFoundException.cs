namespace Puja.Domain.Exceptions
{
    public class PujaNotFoundException : Exception
    {
        public PujaNotFoundException(string message) : base(message)
        {
        }
    }
}