namespace Function.Blending.Opt.Domain.ReadModels;

public sealed record class CalInpParametroItemRm
{
  public Guid CalidadId { get; init; }
  public string? CalidadCodigo { get; init; }
  public string? CalidadNombre { get; init; }
  public Guid ParametroId { get; init; }
  public string? ParametroCodigo { get; set; }
  public string? ParametroNombre { get; set; }
  public decimal? Valor { get; init; }

  public CalInpParametroItemRm() { }
}
