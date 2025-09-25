using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Opt.Infrastructure.Persistence.Support;

public abstract class PooledQueryRepository<TContext>(IDbContextFactory<TContext> factory) where TContext : DbContext
{
  protected readonly IDbContextFactory<TContext> Factory = factory;

  protected async Task<TResult> WithDbAsync<TResult>(Func<TContext, Task<TResult>> action, CancellationToken ct)
  {
    await using var db = await Factory.CreateDbContextAsync(ct);
    return await action(db);
  }

  protected async Task WithDbAsync(Func<TContext, Task> action, CancellationToken ct)
  {
    await using var db = await Factory.CreateDbContextAsync(ct);
    await action(db);
  }
}
