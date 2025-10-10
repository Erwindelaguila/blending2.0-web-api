using System;
using System.Threading;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ISysParamService
{
  Task<Guid?> GetIdAsync(string sysParamKey, CancellationToken ct);
  Task<Guid> GetRequiredIdAsync(string sysParamKey, CancellationToken ct);
}
