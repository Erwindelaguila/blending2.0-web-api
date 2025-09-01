namespace Function.Blending.Auth.Application.Common.Wrappers;

/// <summary>
/// Clase base para respuestas de la aplicación que sigue el patrón Response Wrapper.
/// Proporciona una estructura consistente para todas las respuestas de la API.
/// </summary>
/// <typeparam name="T">Tipo de datos que contiene la respuesta</typeparam>
public class BaseResponse<T>
{
    /// <summary>
    /// Indica si la operación fue exitosa
    /// </summary>
    public bool Succeeded { get; set; }
    
    /// <summary>
    /// Mensaje descriptivo de la operación
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// Lista de errores si la operación falló
    /// </summary>
    public object Errors { get; set; } = new List<string>();
    
    /// <summary>
    /// Datos de la respuesta
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// Código de estado HTTP
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Constructor privado que inicializa las propiedades requeridas
    /// </summary>
    private BaseResponse() 
    { 
        Errors = new List<string>(); // Inicializar para evitar warning CS8618
    }

    /// <summary>
    /// Crea una respuesta exitosa con datos
    /// </summary>
    /// <param name="data">Datos a incluir en la respuesta</param>
    /// <param name="message">Mensaje opcional</param>
    /// <param name="statusCode">Código de estado HTTP (default: 200)</param>
    /// <returns>Respuesta exitosa con datos</returns>
    public static BaseResponse<T> Success(T data, string? message = null, int statusCode = 200)
    {
        return new BaseResponse<T>
        {
            Succeeded = true,
            Data = data,
            Message = message,
            StatusCode = statusCode,
            Errors = new List<string>() // Lista vacía para éxito
        };
    }

    /// <summary>
    /// Crea una respuesta exitosa sin datos
    /// </summary>
    /// <param name="message">Mensaje opcional</param>
    /// <param name="statusCode">Código de estado HTTP (default: 200)</param>
    /// <returns>Respuesta exitosa sin datos</returns>
    public static BaseResponse<T> Success(string? message = null, int statusCode = 200)
    {
        return new BaseResponse<T>
        {
            Succeeded = true,
            Data = default,
            Message = message,
            StatusCode = statusCode,
            Errors = new List<string>() // Lista vacía para éxito
        };
    }

    /// <summary>
    /// Crea una respuesta de error con un mensaje simple
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    /// <param name="statusCode">Código de estado HTTP (default: 400)</param>
    /// <returns>Respuesta de error</returns>
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

    /// <summary>
    /// Crea una respuesta de error con errores múltiples
    /// </summary>
    /// <param name="errors">Lista de errores</param>
    /// <param name="message">Mensaje principal opcional</param>
    /// <param name="statusCode">Código de estado HTTP (default: 400)</param>
    /// <returns>Respuesta de error con múltiples errores</returns>
    public static BaseResponse<T> Fail(object errors, string? message = null, int statusCode = 400)
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
