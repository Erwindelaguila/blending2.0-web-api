using MediatR;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;

public class CreateTipoProduccionCommand : IRequest<TipoProduccionDTO>
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public Guid LineaProduccionId { get; set; }
    public Guid AgregadoId { get; set; }
    public bool? Activo { get; set; }
    public Guid CreadoPorId { get; set; }
}
