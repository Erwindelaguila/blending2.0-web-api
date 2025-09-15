namespace Function.Blending.Upload.Models;

public class ExcelExtractQualityDto
{
    public ValoresFijosDto Fijos { get; set; } = new(); // Usamos directamente la clase POCO
    public Dictionary<string, string> ParametrosCalidad { get; set; } = new();
    public Dictionary<string, string> OtrosValores { get; set; } = new();
}

public class ValoresFijosDto
{
    public string RumaNro { get; set; }
    public string Planta { get; set; }
    public string Anio { get; set; }
    public string Serie { get; set; }
    public string FechaCorte { get; set; }
    public string DescripcionCalidad { get; set; }
    public string NombreCalidad { get; set; }
    public double Cantidad { get; set; }
    public string Um { get; set; }
    public string Codigo { get; set; }
    public string DescripcionMaterial { get; set; }
    public string CentroUbicacion { get; set; }
    public string AlmacenUbicacion { get; set; }
    public string TipoProduccion { get; set; }
    public string CentroProduccion { get; set; }
    public string CalidadPlanta { get; set; }
    public string CierreVta { get; set; }
    public int Posicion { get; set; }
    public string Material { get; set; }
    public int CantPreAsignado { get; set; }
    public int CantTransito { get; set; }
    public int CantLote { get; set; }
    public string LoteExp { get; set; }
    public string UbicacionEnAlmacen { get; set; }
    public string FechaContabilizacion { get; set; }
    public string FechaFabricacion { get; set; }
    public string Certificadora { get; set; }
  
    // posibles fechas
    public string FAnalFcoQco { get; set; }
    public string FAnalMicobiol { get; set; }
    public string FvAnalFcoQco { get; set; }
    public string FvAnalMocobiol { get; set; }
}