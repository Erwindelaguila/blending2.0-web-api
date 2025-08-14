namespace Function.Blending.Upload.Models;

public class ReporteLogisticDto
{
    public Demanda Demanda { get; set; } = new Demanda();
    public Dictionary<string, Oferta> Oferta { get; set; } = new Dictionary<string, Oferta>();
    public string Contrato { get; set; } = string.Empty;
    public string PesoContenedores { get; set; }= string.Empty;
}

public class Demanda
{
    public DemandaFijas Fijos { get; set; } = new DemandaFijas();
    public Dictionary<string, string> ParamentrosCalidad { get; set; } = new Dictionary<string, string>();
}

public class Oferta
{
    public OfertaFijas Fijos { get; set; } = new OfertaFijas();
    public Dictionary<string, string> ParametrosCalidad { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> OtrosParamentros { get; set; } = new Dictionary<string, string>();
}


public class DemandaFijas
{
    public int Pos { get; set; }
    public string Material { get; set; }
    public string Descripcion { get; set; }
    public double CantidadAsignada { get; set; }
    public string UMVta { get; set; }
    public double CantidadAsignadaTemp { get; set; }
    public string UMAlmac { get; set; }
    public double Tolerancia { get; set; }
}

public class OfertaFijas
{
    public int Pos { get; set; }
    public string DescripcionMaterial { get; set; }
    public string DescripcionCentro { get; set; }
    public string Lote { get; set; }
    public double CantidadAsignada { get; set; }
    public string UMAlmac { get; set; }
    public string FechaCotizacion { get; set; }
    public string FechaFabricacion { get; set; }
    public double CantidadAsignadaTemp { get; set; }
    public string UMVta { get; set; }
    public string FechaAnalisisQuimico { get; set; }
    public string FechaVencimientoQuimico { get; set; }
    public string FechaAnalisisMicro { get; set; }
    public string FechaVencimientoMicro { get; set; }
    public string TipoAlmacen { get; set; }
    public string UbicacionAlmacen { get; set; }
}
