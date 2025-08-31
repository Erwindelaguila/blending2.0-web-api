namespace Function.Blending.Core.Application.CalidadParametro.DTOs;

public class CalidadParametroItemDTO
{
    public Guid Id { get; set; }
    public Guid CalidadId { get; set; }
    public string CalidadCodigo { get; set; } = string.Empty;
    public string CalidadNombre { get; set; } = string.Empty;
    public Guid ParametroId { get; set; }
    public string ParametroCodigo { get; set; } = string.Empty;
    public string ParametroNombre { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public bool EsDefault { get; set; } 
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
}
