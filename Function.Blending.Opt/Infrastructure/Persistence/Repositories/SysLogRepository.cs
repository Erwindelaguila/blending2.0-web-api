using System;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Infrastructure.Persistence;
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models;

namespace Function.Blending.Opt.Infrastructure.Persistence.Repositories;

public sealed class SysLogRepository(BlendingDbContext db) : ISysLogRepository
{
  public async Task WriteAsync(SysLogRecord entry, CancellationToken ct)
  {
    // Map explícito Domain -> EF (sin AutoMapper para minimizar deps)
    var row = new EF.SysLog
    {
      Id = entry.Id,
      NameSpace = entry.NameSpace,
      ClassName = entry.ClassName,
      MethodName = entry.MethodName,
      Username = entry.Username,
      UserId = entry.UserId,
      DateTime = DateTime.UtcNow,
      Message = entry.Message,
      StackTrace = entry.StackTrace,
      ExtraInfo = entry.ExtraInfo,
      RequestInvocationId = entry.RequestInvocationId,
      FunctionInvocationId = entry.FunctionInvocationId,
      ExceptionGroupId = entry.ExceptionGroupId,
      Level = entry.Level.ToDbString() // "debug" | "info" | "warning" | "error"
    };

    db.Set<EF.SysLog>().Add(row);
    await db.SaveChangesAsync(ct);
  }
}
