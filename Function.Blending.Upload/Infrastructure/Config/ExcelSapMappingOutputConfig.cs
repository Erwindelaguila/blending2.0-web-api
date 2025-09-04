namespace Function.Blending.Upload.Infrastructure.Config;

public class ExcelSapMappingOutputConfig
{
    public string Version { get; set; }
    public Data Data { get; set; }
}

public class Data
{
    public string SheetName { get; set; }
    public int StartRow { get; set; }
    public Dictionary<string, CampoExcel>  Fijos { get; set; }
    public Dictionary<string, CampoExcel> ParametrosCalidad { get; set; }
    public Dictionary<string, CampoExcel> OtrosParametros { get; set; }
}

public class CampoExcel
{
    public string Header { get; set; }
    public string Column { get; set; }
}
