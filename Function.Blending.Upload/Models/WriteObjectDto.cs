using System.Text.Json.Nodes;

namespace Function.Blending.Upload.Models;

public class WriteObjectDto
{ 
    public Resultado resultado { get; set; }
    public string status { get; set; } 
}

public class Resultado
{
    public JsonObject grupos { get; set; }
    public JsonObject rumas { get; set; } 
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
