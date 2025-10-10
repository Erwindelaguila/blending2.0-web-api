namespace Function.Blending.Upload.Models;

public class ExcelExtractLogisticDto
{
    public Demanda Demanda { get; set; } = new Demanda();
    public Dictionary<string, Oferta> Oferta { get; set; } = new Dictionary<string, Oferta>();
    public string Contrato { get; set; } = string.Empty;
    public string PesoContenedores { get; set; }= string.Empty;
    public string PedidoVenta { get; set; } = string.Empty;
    public string FechaCarguio { get; set; } = string.Empty;
    public string PlantaCodigo { get; set; } = string.Empty;
    public string PlantaDescripcion { get; set; } = string.Empty;
    public string AlmacenCodigo { get; set; } = string.Empty;
    public string AlmacenDescripcion { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string Asistente { get; set; } = string.Empty;
    public string Supervisora { get; set; } = string.Empty;
    public string PaisDestino  { get; set; } = string.Empty;
    public string CantidadRuma  { get; set; } = string.Empty;
    public string UnidadMedidaRuma { get; set; } = string.Empty;
    public string NumeroMovimientos { get; set; } = string.Empty;
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
    public double CantidadAsignadaVenta { get; set; }
    public string UnidadMedidaVenta { get; set; }
    public double CantidadAsignadaAlmacen { get; set; }
    public string UnidadMedidaAlmacen { get; set; }
    public double Tolerancia { get; set; }
}

public class OfertaFijas
{
    public int Pos { get; set; }
    public string DescripcionMaterial { get; set; }
    public string DescripcionCentro { get; set; }
    public string Lote { get; set; }
    public double CantidadAsignadaSacos { get; set; }
    public string UMAlmac { get; set; }
    public string FechaCotizacion { get; set; }
    public string FechaFabricacion { get; set; }
    public double CantidadAsignadaToneladas { get; set; }
    public string UMVta { get; set; }
    public string FechaAnalisisQuimico { get; set; }
    public string FechaVencimientoQuimico { get; set; }
    public string FechaAnalisisMicro { get; set; }
    public string FechaVencimientoMicro { get; set; }
    public string TipoAlmacen { get; set; }
    public string UbicacionAlmacen { get; set; }
}
