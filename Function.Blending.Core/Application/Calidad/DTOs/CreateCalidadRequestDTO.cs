using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Calidad.DTOs;

public class CreateCalidadRequestDTO
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string CodigoMaterial { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool? NoConforme { get; set; }
    public bool? Activo { get; set; }
}
