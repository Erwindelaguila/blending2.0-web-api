namespace Function.Blending.Opt.Shared.Security;

public interface IHmacKeyResolver
{
  /// <summary>Devuelve la clave en bytes para el keyId; lanza si no existe.</summary>
  Task<ReadOnlyMemory<byte>> ResolveAsync(string keyId, CancellationToken ct = default);
}
