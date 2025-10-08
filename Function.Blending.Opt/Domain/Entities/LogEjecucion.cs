using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Domain.ValueObjects.Ids;
using System;

namespace Function.Blending.Opt.Domain.Entities;

public sealed class LogEjecucion
{
  public EjecucionId Id { get; set; }

  public EstadoId EstadoId { get; private set; }

  public string? Codigo { get; set; }
  public bool? Confirmado { get; set; }
  public string? Mensaje { get; set; }
  public string? NombreArchivo { get; set; }
  public string? UrlArchivo { get; set; }
  public DateTime CreadoEl { get; set; }   // UTC

  public string? EstadoNombre { get; set; }
  public EstadoLogisticaSnapshot? Estado { get; set; }

  public LogEjecucion() { }
  public LogEjecucion(EjecucionId id) => Id = id;

  public LogInpInfo? Info { get; set; }
  public LogInpFiltro? Filtro { get; set; }
  public LogInpDemanda? Demanda { get; set; }
  public IReadOnlyList<LogInpOferta>? Oferta { get; set; }
  public IReadOnlyList<LogOutContenedor>? Contenedores { get; set; }
}
