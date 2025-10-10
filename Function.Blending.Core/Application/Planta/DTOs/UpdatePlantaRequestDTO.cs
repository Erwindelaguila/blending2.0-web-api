using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Planta.DTOs;

public class UpdatePlantaRequestDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int NumeroRuma { get; set; }
    public bool? Activo { get; set; }
}
