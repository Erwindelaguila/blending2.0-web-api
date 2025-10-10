namespace Function.Blending.Upload.Infrastructure.Config.Output;

public class ExcelMappingOutputLogisticsConfig
{
    public string Version { get; set; }
    public Homogenizacion Homogenizacion { get; set; }
    public Sap Sap { get; set; }
}

public class Sap
{
    public string SheetName { get; set; }
    public int StartRow { get; set; }
}
public class Homogenizacion
{
    public string SheetName { get; set; }
    public int StartRow { get; set; }
    public Dictionary<string, Puerto> Puertos { get; set; }
    public Dictionary<string, ParametroCalidad> PametrosCalidad  { get; set; }
}

public class Puerto
{
    public string Column { get; set; }
    public string Nombre { get; set; }
    public int Soporte { get; set; }
    public string CodeAfla { get; set; }
}

public class ParametroCalidad
{
    public string Header { get; set; }
    public string Name { get; set; }
    public double Cantidad { get; set; }
}