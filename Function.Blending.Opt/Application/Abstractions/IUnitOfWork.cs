using System.Threading;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Application.Abstractions;

[Obsolete("No se usa. Transacciones locales en repos. Mantener solo como referencia.")]
public interface IUnitOfWork
{
  Task<int> SaveChangesAsync(CancellationToken ct = default);
}
