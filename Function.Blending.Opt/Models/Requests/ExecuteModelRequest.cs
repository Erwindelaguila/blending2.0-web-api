namespace Function.Blending.Opt.Models.Requests;

public class ExecuteModelRequest
{
    public string Planta { get; set; }
    public string TipoModelo { get; set; } // "Calidad" o "Logistica"
    public Dictionary<string, object> Parametros { get; set; }
}