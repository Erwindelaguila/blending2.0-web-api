using Microsoft.Azure.Functions.Worker;
using System.Threading;

namespace Function.Blending.Core.Functions.Support.Execution;

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
