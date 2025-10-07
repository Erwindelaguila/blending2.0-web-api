using Function.Blending.Core.Application.Common.Wrappers;

namespace Function.Blending.Core.Application.Parametro.DTOs;

public class ParametroDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
}

public class ParametroShortDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
}

public class ParametroResponseDTO
{
    public PagedResponse<ParametroDTO>? ParametroPaginate { get; set; } = null!;
    public List<ParametroShortDTO>? ParametroShortList { get; set; } = null!;
}
