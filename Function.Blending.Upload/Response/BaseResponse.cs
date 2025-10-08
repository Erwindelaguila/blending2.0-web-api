namespace Function.Blending.Upload.Response;

public class BaseResponse<T>
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public object? Errors { get; set; }
    public T? Data { get; set; }
    public int StatusCode { get; set; }

    private BaseResponse() { }

    // Éxito con datos
    public static BaseResponse<T> Success(T data, string? message = null, int statusCode = 200)
    {
        return new BaseResponse<T>
        {
            Succeeded = true,
            Data = data,
            Message = message,
            StatusCode = statusCode
        };
    }

    // Éxito sin datos
    public static BaseResponse<T> Success(string? message = null, int statusCode = 200)
    {
        return new BaseResponse<T>
        {
            Succeeded = true,
            Data = default,
            Message = message,
            StatusCode = statusCode
        };
    }

    // Error simple
    public static BaseResponse<T> Fail(string message, int statusCode = 400)
    {
        return new BaseResponse<T>
        {
            Succeeded = false,
            Message = message,
            StatusCode = statusCode,
            Errors = new List<string> { message }
        };
    }

    // Error múltiple
    public static BaseResponse<T> Fail(object? errors, string? message = null, int statusCode = 400)
    {
        return new BaseResponse<T>
        {
            Succeeded = false,
            Message = message ?? "Ocurrieron errores de validación",
            Errors = errors,
            StatusCode = statusCode
        };
    }
}