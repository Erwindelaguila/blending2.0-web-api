using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogInpInfo
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public string Contrato { get; set; } = null!;

    public string PedidoVenta { get; set; } = null!;

    public DateTime FechaCarguio { get; set; }

    public string PlantaCodigo { get; set; } = null!;

    public string PlantaDescripcion { get; set; } = null!;

    public string AlmacenCodigo { get; set; } = null!;

    public string AlmacenDescripcion { get; set; } = null!;

    public string Cliente { get; set; } = null!;

    public string Asistente { get; set; } = null!;

    public string Supervisora { get; set; } = null!;

    public string PaisDestino { get; set; } = null!;

    public int CantidadRuma { get; set; }

    public string UnidadMedidaRuma { get; set; } = null!;

    public int NumeroMovimientos { get; set; }

    public virtual LogEjecucion Ejecucion { get; set; } = null!;
}
