namespace Function.Blending.Core.Application.CalidadParametro.DTOs;

// DTO para la matriz completa
public class CalidadParametroMatrizDTO
{
    public List<ParametroInfo> Parametros { get; set; } = new();
    public List<CalidadInfo> Calidades { get; set; } = new();
}

public class ParametroInfo
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
}

public class CalidadInfo
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public Dictionary<string, ValorInfo> Valores { get; set; } = new();
}

public class ValorInfo
{
    public decimal Valor { get; set; }
    public bool EsDefault { get; set; }
}

// DTO para actualizar una celda individual
public class UpsertCalidadParametroDTO
{
    public Guid CalidadId { get; set; }
    public Guid ParametroId { get; set; }
    public decimal Valor { get; set; }
    public Guid ModificadoPorId { get; set; }
}

// DTO para actualizar múltiples celdas de una vez (ARRAY)
public class UpsertCalidadParametroBatchDTO
{
    public List<CalidadParametroCambioDTO> Cambios { get; set; } = new();
    public Guid ModificadoPorId { get; set; }
}

public class CalidadParametroCambioDTO
{
    public Guid CalidadId { get; set; }
    public Guid ParametroId { get; set; }
    public decimal Valor { get; set; }
}
