namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses
{
  public sealed class StartLogEjecucionResponse
  {
    public Guid Id { get; init; }
    public Guid PlantaId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public DateTime CreadoEl { get; init; } // UTC
    public EstadoResponse? Estado { get; init; }
  }
}
