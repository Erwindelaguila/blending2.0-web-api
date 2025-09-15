namespace Function.Blending.Upload.Infrastructure.Config.Output;

public class ExcelMappingOutputQualityConfig
{
    public string Version { get; set; }
    public Resumen Resumen { get; set; }
    public Detalle Detalle { get; set; }
    public Paramentros Parametros { get; set; }
}


public class Resumen
{
    public string SheetName { get; set; }
    public int StartRow { get; set; }
    public Dictionary<string, ExcelConfig> Fijos { get; set; }
    public Dictionary<string, ExcelConfig> ParametrosCalidad { get; set; }
}

public class Detalle
{
    public string SheetName { get; set; }
    public int StartRow { get; set; }
    public Dictionary<string, ExcelConfig> Fijos { get; set; }
    public Dictionary<string, ExcelConfig> ParametrosCalidad { get; set; }
}

public class Paramentros
{
    public string SheetName { get; set; }
    public int StartRow { get; set; }
    public Dictionary<string, ExcelConfig> Fijos { get; set; }
}

public class ExcelConfig
{
    public string Header { get; set; }
    public string Column { get; set; }
    public string DataType { get; set; }
    
}