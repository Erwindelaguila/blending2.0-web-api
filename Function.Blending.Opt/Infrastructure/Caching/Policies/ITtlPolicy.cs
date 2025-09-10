namespace Function.Blending.Opt.Infrastructure.Caching.Policies;

public interface ITtlPolicy<TKey>
{
  int Resolve(TKey key);
}
