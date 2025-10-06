using Microsoft.EntityFrameworkCore;

public interface IDbContextRunner<T> where T : DbContext
{
  Task<TResult> RunAsync<TResult>(Func<T, Task<TResult>> action, CancellationToken ct);
}

public sealed class DbContextRunner<T>(IDbContextFactory<T> factory) : IDbContextRunner<T> where T : DbContext
{
  public async Task<TResult> RunAsync<TResult>(Func<T, Task<TResult>> action, CancellationToken ct)
  {
    await using var db = await factory.CreateDbContextAsync(ct);
    return await action(db);
  }
}
