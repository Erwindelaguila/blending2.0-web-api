namespace Function.Blending.Upload.Models;

public class CalidadDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string CodigoMaterial { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool NoConforme { get; set; }
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
}

public class ApiResponseDto<T>
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Errors { get; set; }
    public List<T> Data { get; set; } = new();
    public int StatusCode { get; set; }
}