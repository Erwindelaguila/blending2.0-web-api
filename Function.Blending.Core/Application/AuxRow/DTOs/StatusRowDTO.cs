namespace Function.Blending.Core.Application.AuxRow.DTOs;

public class StatusRowDTO
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; }
}