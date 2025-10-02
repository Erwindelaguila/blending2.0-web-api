namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record LogOutConComposicion(
  Guid? Id = default,
  Guid? ContenedorId = default,
  string? CodigoParametro = null,
  decimal? Valor = null
);