namespace Function.Blending.Upload.Infrastructure.Config;

public class  ExcelMappingLogisticsConfig
{
    public string Version { get; set; }
    public string SheetName { get; set; }
    public string Contrato { get; set; }
    public int StartRowOferta { get; set; }
    public string ColumCantidadSacos { get; set; }
    public Demanda Demanda { get; set; }
    public Oferta Oferta { get; set; }
}


public class Demanda
{
    public DemandaColumnasFijasConfig Fijos { get; set; }
    public Dictionary<string, string> ParamentrosCalidad { get; set; }
}

public class DemandaColumnasFijasConfig
{
    public string Pos { get; set; }
    public string Material { get; set; }
    public string Descripcion { get; set; }
    public string CantidadAsignada { get; set; }
    public string UMVta { get; set; }
    public string CantidadAsignadaTemp { get; set; }
    public string UMAlmac { get; set; }
    public string Tolerancia { get; set; }
}


public class Oferta
{
    public OfertaColumnasFijasConfig Fijos { get; set; }
    public Dictionary<string, string> ParametrosCalidad { get; set; }
    public Dictionary<string, string> OtrosParamentros { get; set; }
}

public class OfertaColumnasFijasConfig
{
    public string Pos { get; set; }
    public string DescripcionMaterial { get; set; }
    public string DescripcionCentro { get; set; }
    public string Lote { get; set; }
    public string CantidadAsignada { get; set; }
    public string UMAlmac { get; set; }
    public string FechaCotizacion { get; set; }
    public string FechaFabricacion { get; set; }
    public string CantidadAsignadaTemp { get; set; }
    public string UMVta { get; set; }
    public string FechaAnalisisQuimico { get; set; }
    public string FechaVencimientoQuimico { get; set; }
    public string FechaAnalisisMicro { get; set; }
    public string FechaVencimientoMicro { get; set; }
    public string TipoAlmacen { get; set; }
    public string UbicacionAlmacen { get; set; }
}
