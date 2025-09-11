using System;
using AutoMapper;
using Function.Blending.Opt.Infrastructure.Persistence.Constants;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings
{
  public static class MappingExtensions
  {
    // ===== ExecutionId (ya existente) =====
    public static IMappingOperationOptions UseExecutionId(this IMappingOperationOptions opts, Guid ejecucionId)
    {
      opts.Items[MappingKeys.ExecutionId] = ejecucionId;
      return opts;
    }

    public static Guid GetExecutionId(this ResolutionContext ctx)
    {
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));
      if (ctx.Items != null && ctx.Items.TryGetValue(MappingKeys.ExecutionId, out var val) && val is Guid id)
        return id;

      throw new InvalidOperationException($"{MappingKeys.ExecutionId} no presente en Mapping Options Items.");
    }

    // ===== UserId (nuevo) =====
    public static IMappingOperationOptions UseUserId(this IMappingOperationOptions opts, Guid userId)
    {
      opts.Items[MappingKeys.UserId] = userId;
      return opts;
    }

    public static Guid GetUserId(this ResolutionContext ctx)
    {
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));
      if (ctx.Items != null && ctx.Items.TryGetValue(MappingKeys.UserId, out var val) && val is Guid id)
        return id;

      throw new InvalidOperationException($"{MappingKeys.UserId} no presente en Mapping Options Items.");
    }

    // ===== ParentId (nuevo) =====
    public static IMappingOperationOptions UseParentId(this IMappingOperationOptions opts, Guid parentId)
    {
      opts.Items[MappingKeys.ParentId] = parentId;
      return opts;
    }

    public static Guid GetParentId(this ResolutionContext ctx)
    {
      if (ctx == null) throw new ArgumentNullException(nameof(ctx));
      if (ctx.Items != null && ctx.Items.TryGetValue(MappingKeys.ParentId, out var val) && val is Guid id)
        return id;

      throw new InvalidOperationException($"{MappingKeys.ParentId} no presente en Mapping Options Items.");
    }
  }
}
