using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Parametro.DTOs;

public class UpdateParametroRequestDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool? Activo { get; set; }
}
