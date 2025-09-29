using System.Text.Json.Nodes;

namespace Function.Blending.Upload.Models;

public class WriteObjectDto
{ 
    public List<Dictionary<string, object>> grupos { get; set; }
    public List<Dictionary<string, object>> detalles { get; set; } 
}


public class WriteLogisticObjectDto
{
    public JsonObject Data  { get; set; }
    public Metadata Metadata { get; set; }
}

public class Metadata
{
    public string Timestamp { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public string ErrorCode { get; set; }
}
