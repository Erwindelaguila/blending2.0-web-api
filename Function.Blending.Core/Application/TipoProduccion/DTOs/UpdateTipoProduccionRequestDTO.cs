using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.TipoProduccion.DTOs;

/// <summary>
/// DTO para requests de actualización de tipo de producción
/// </summary>
public class UpdateTipoProduccionRequestDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public Guid LineaProduccionId { get; set; }
    public Guid AgregadoId { get; set; }
    public bool? Activo { get; set; } = true;
}
