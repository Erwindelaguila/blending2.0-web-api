namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Input;

public sealed class CalInpFiltroDto
{
  public string? CentroUbicacion { get; set; }
  public string? CentroProduccion { get; set; }
  public string? UbicacionAlmacen { get; set; }
  public bool MezclarTipoProduccion { get; set; }
  public string? TipoProduccion { get; set; }
  public string? BorrarCalidades { get; set; }
  public bool QuitarRumasPH { get; set; }
  public string? AgregarRumasSerie { get; set; }
  public bool ConsiderarCadmio { get; set; }
  public DateTimeOffset? FechaCorte { get; set; }
  public int? NumeroRuma { get; set; }
  public int? DivisionRuma { get; set; }
  public decimal ValorCadmioAlto { get; set; }
}
