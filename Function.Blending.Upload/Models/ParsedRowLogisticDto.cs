namespace Function.Blending.Upload.Models;

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
    public string PedidoVenta { get; set; } = string.Empty;
    public string FechaCarguio { get; set; } = string.Empty;
    public string PlantaCodigo { get; set; } = string.Empty;
    public string PlantaDescripcion { get; set; } = string.Empty;
    public string AlmacenCodigo { get; set; } = string.Empty;
    public string AlmacenDescripcion { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string Asistente { get; set; } = string.Empty;
    public string Supervisora { get; set; } = string.Empty;
    public string PaisDestino  { get; set; } = string.Empty;
    public string CantidadRuma  { get; set; } = string.Empty;
    public string UnidadMedidaRuma { get; set; } = string.Empty;
    public string NumeroMovimientos { get; set; } = string.Empty;
    public string PesoContenedores { get; set; } = string.Empty;
}

public class ParsedOfertaDto
{
    public Dictionary<string, string> Fijos { get; set; } = new();
    public Dictionary<string, string> ParametrosCalidad { get; set; } = new();
    public Dictionary<string, string> OtrosParametros { get; set; } = new();
}