namespace Function.Blending.Upload.Models;

public class ParsedRowQualityDto
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



