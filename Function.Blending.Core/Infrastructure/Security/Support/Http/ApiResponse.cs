namespace Function.Blending.Core.Infrastructure.Security.Support.Http;

public sealed class ApiResponse<T>
{
  public bool Success { get; init; }
  public T? Data { get; init; }
  public object? Meta { get; init; }

  public static ApiResponse<T> Of(T data, object? meta = null) => new() { Success = true, Data = data, Meta = meta };
}
