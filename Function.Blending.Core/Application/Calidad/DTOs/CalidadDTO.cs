using Function.Blending.Core.Application.Common.Wrappers;

namespace Function.Blending.Core.Application.Calidad.DTOs;

using System;

public class CalidadDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? CodigoMaterial { get; set; }
    public string? Descripcion { get; set; }
    public bool NoConforme { get; set; }
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
}


public class CalidadesResponseDTO
{
    public PagedResponse<CalidadDTO>? CalidadPaginate { get; set; } = null!;
    public List<CalidadDTO>? CalidadList { get; set; } = null!;
}