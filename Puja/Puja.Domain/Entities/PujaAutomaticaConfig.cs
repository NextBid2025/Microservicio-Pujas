namespace Puja.Domain.Entities;

public class PujaAutomaticaConfig
{
    public string Id { get; set; }
    public string SubastaId { get; set; }
    public string UserId { get; set; }
    public decimal MontoMaximo { get; set; }
    public DateTime FechaCreacion { get; set; }
}