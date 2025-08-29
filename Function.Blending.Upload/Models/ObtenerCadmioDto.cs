using System.Text.Json.Serialization;

namespace Function.Blending.Upload.Models;

public class ObtenerCadmioRequestDto
{
    public string[] Rumas { get; set; } = Array.Empty<string>();
}

public class ObtenerCadmioSapRequestDto
{
    
    public ZsdfBlendingGetCadmio ZSDF_BLENDING_GET_CADMIO { get; set; } = new();
}

public class ZsdfBlendingGetCadmio
{
    public ItRumas IT_RUMAS { get; set; } = new();
}

public class ItRumas
{
    public List<ItemRuma> item { get; set; } = new();
}

public class ItemRuma
{
    public string CHARG { get; set; } = string.Empty;
}

public class CadmioResult
{
    [JsonPropertyName("rumaNro")]
    public string RumaNro { get; set; } = string.Empty;
    [JsonPropertyName("valor")]
    public string Valor { get; set; }
}

public class ObtenerCadmioResponseDto
{
    [JsonPropertyName("data")]
    public List<CadmioResult> Data { get; set; } = new();
}

public class ObtenerCadmioSapResponseDto
{
    [JsonPropertyName("n0:ZSDF_BLENDING_GET_CADMIOResponse")]
    public ZsdfBlendingGetCadmioResponse ZSDF_BLENDING_GET_CADMIOResponse { get; set; }
}

public class ZsdfBlendingGetCadmioResponse
{
    public EtData ET_DATA { get; set; }
}

public class EtData
{
    public List<ItemCadmio> item { get; set; } = new();
}

public class ItemCadmio
{
    public string CHARG { get; set; } = string.Empty;
    public string CADMIO { get; set; } = string.Empty;
    public string CODMUESTRA { get; set; } = string.Empty;
}
