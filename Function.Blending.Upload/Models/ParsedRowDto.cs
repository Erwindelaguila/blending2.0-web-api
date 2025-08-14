namespace Function.Blending.Upload.Models;

public class ParsedRowDto
{
    /// <summary>
    /// Contiene las columnas fijas del Excel.
    /// </summary>
    public Dictionary<string, string> Fijos { get; set; } = new();
  
    /// <summary>
    /// Contiene los parámetros de calidad dinámicos.
    /// </summary>
    public Dictionary<string, string> ParametrosCalidad { get; set; } = new();
  
    /// <summary>
    /// Contiene los otros valores dinámicos.
    /// </summary>
    public Dictionary<string, string> OtrosValores { get; set; } = new();
  
}


public class ParsedRowDemandaDto
{
    public Dictionary<string, string> Fijos { get; set; } = new();
    public Dictionary<string, string> ParametrosCalidad { get; set; } = new();
}

public class ParsedRowLogisticDto
{
    public ParsedRowDemandaDto Demanda { get; set; } = new();
    public  Dictionary<string,ParsedOfertaDto> Oferta { get; set; } = new();
    public string Contrato { get; set; } = string.Empty;
    public string PesoContenedores { get; set; } = string.Empty;
}

public class ParsedOfertaDto
{
    public Dictionary<string, string> Fijos { get; set; } = new();
    public Dictionary<string, string> ParametrosCalidad { get; set; } = new();
    public Dictionary<string, string> OtrosParametros { get; set; } = new();
}
