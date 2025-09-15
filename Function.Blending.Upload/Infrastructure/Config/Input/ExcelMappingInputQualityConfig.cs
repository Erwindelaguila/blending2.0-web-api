namespace Function.Blending.Upload.Infrastructure.Config.Input;

public class ExcelMappingInputQualityConfig
{
    public string Version { get; set; } = "1.0";
    public string SheetName { get; set; } = string.Empty;
    public int StartRow { get; set; }

    public ColumnasFijasConfig Fijos { get; set; } = new();
    public Dictionary<string, string> ParametrosCalidad { get; set; } = new();
    public Dictionary<string, string> OtrosValores { get; set; } = new();
}

public class ColumnasFijasConfig
{
    public string RumaNro { get; set; }
    public string Cantidad { get; set; }
    public string Um { get; set; }
    public string Codigo { get; set; }
    public string DescripcionMaterial { get; set; }
    public string CentroUbicacion { get; set; }
    public string AlmacenUbicacion { get; set; }
    public string TipoProduccion { get; set; }
    public string CentroProduccion { get; set; }
    public string CalidadPlanta { get; set; }
    public string CierreVta { get; set; }
    public string Posicion { get; set; }
    public string Material { get; set; }
    public string CantPreAsignado { get; set; }
    public string CantTransito { get; set; }
    public string CantLote { get; set; }
    public string LoteExp { get; set; }
    public string UbicacionEnAlmacen { get; set; }
    public string FechaContabilizacion { get; set; }
    public string FechaFabricacion { get; set; }
    public string Certificadora { get; set; }
    public string FAnalFcoQco { get; set; }
    public string FAnalMicobiol { get; set; }
    public string FvAnalFcoQco { get; set; }
    public string FvAnalMocobiol { get; set; }
}
