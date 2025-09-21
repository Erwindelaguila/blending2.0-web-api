using Microsoft.Azure.Functions.Worker;

namespace Function.Blending.Core.Infrastructure.Security.Support.Execution;

public interface IFunctionContextAccessor
{
  FunctionContext? Current { get; set; }
}

public sealed class FunctionContextAccessor : IFunctionContextAccessor
{
  private static readonly AsyncLocal<FunctionContext?> _holder = new();
  public FunctionContext? Current
  {
    get => _holder.Value;
    set => _holder.Value = value;
  }
}
