namespace Function.Blending.Opt.Shared.Results
{
  public enum ResultKind
  {
    None = 0,
    Validation = 1,
    NotFound = 2,
    Unauthorized = 3,
    Forbidden = 4,
    Conflict = 5
  }

  /// <summary>Result pattern minimalista con tipado básico de error.</summary>
  public class Result
  {
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ResultKind Kind { get; }

    protected Result(bool ok, string? error, ResultKind kind = ResultKind.None)
      => (IsSuccess, Error, Kind) = (ok, error, kind);

    public static Result Ok() => new(true, null, ResultKind.None);
    public static Result Fail(string error) => new(false, error, ResultKind.Validation);
    public static Result NotFound(string error = "Not Found") => new(false, error, ResultKind.NotFound);

    // Flags de conveniencia
    public bool IsNotFound => !IsSuccess && Kind == ResultKind.NotFound;
  }

  public sealed class Result<T> : Result
  {
    public T? Value { get; }

    private Result(bool ok, T? value, string? error, ResultKind kind)
      : base(ok, error, kind) => Value = value;

    public static Result<T> Ok(T value) => new(true, value, null, ResultKind.None);
    public static new Result<T> Fail(string error) => new(false, default, error, ResultKind.Validation);
    public static new Result<T> NotFound(string error = "Not Found") => new(false, default, error, ResultKind.NotFound);
  }
}
