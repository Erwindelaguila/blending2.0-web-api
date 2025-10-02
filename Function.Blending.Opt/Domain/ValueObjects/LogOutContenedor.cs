namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record LogOutContenedor(
  Guid? Id = default,
  Guid? EjecucionId = default,
  string? Contenedor = null,
  string? Grupo = null,
  Guid? CreadoPorId = null,
  DateTime? CreadoEl = null,
  Guid? ModificadoPorId = null,
  DateTime? ModificadoEl = null,
  IReadOnlyList<LogOutConComposicion>? LogOutConComposicion = null,
  IReadOnlyList<LogOutConDistribucion>? LogOutConDistribucion = null
);