namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record LogOutConDistribucion(
  Guid? Id = default,
  Guid? ContenedorId = default,
  string? Ruma = null,
  decimal? Valor = null
);
