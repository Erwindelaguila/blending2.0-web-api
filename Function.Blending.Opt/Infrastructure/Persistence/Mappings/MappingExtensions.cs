using System;
using AutoMapper;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings
{
  public static class MappingExtensions
  {
    // ===== ExecutionId (ya existente) =====
    public static IMappingOperationOptions UseExecutionId(this IMappingOperationOptions opts, Guid ejecucionId)
    {
      opts.Items["EjecucionId"] = ejecucionId;
      return opts;
    }

    public static Guid GetExecutionId(this ResolutionContext ctx)
    {
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));
      if (ctx.Items != null && ctx.Items.TryGetValue("EjecucionId", out var val) && val is Guid id)
        return id;

      throw new InvalidOperationException("EjecucionId no presente en Mapping Options Items.");
    }

    // ===== UserId (nuevo) =====
    public static IMappingOperationOptions UseUserId(this IMappingOperationOptions opts, Guid userId)
    {
      opts.Items["UserId"] = userId;
      return opts;
    }

    public static Guid GetUserId(this ResolutionContext ctx)
    {
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));
      if (ctx.Items != null && ctx.Items.TryGetValue("UserId", out var val) && val is Guid id)
        return id;

      throw new InvalidOperationException("UserId no presente en Mapping Options Items.");
    }

    // ===== ParentId (nuevo) =====
    public static IMappingOperationOptions UseParentId(this IMappingOperationOptions opts, Guid parentId)
    {
      opts.Items["ParentId"] = parentId;
      return opts;
    }

    public static Guid GetParentId(this ResolutionContext ctx)
    {
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));
      if (ctx.Items != null && ctx.Items.TryGetValue("ParentId", out var val) && val is Guid id)
        return id;

      throw new InvalidOperationException("ParentId no presente en Mapping Options Items.");
    }
  }
}
