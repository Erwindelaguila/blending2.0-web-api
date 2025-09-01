using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Agregado.DTOs;

public class UpdateAgregadoRequestDTO
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool? Activo { get; set; } = true;
}
