using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogInpOferta
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public string Ruma { get; set; } = null!;

    public int Posicion { get; set; }

    public string DescripcionMaterial { get; set; } = null!;

    public string DescripcionCentro { get; set; } = null!;

    public decimal CantidadAsignadaAlmacen { get; set; }

    public string? UnidadMedidaAlmacen { get; set; }

    public string FechaContabilizacion { get; set; } = null!;

    public string FechaFabricacion { get; set; } = null!;

    public decimal CantidadAsignadaVenta { get; set; }

    public string? UnidadMedidaVenta { get; set; }

    public string? FechaAnalisisQuimico { get; set; }

    public string? FechaVencimientoQuimico { get; set; }

    public string? FechaAnalisisMicrobiologico { get; set; }

    public string? FechaVencimientoMicrobiologico { get; set; }

    public string? TipoAlmacen { get; set; }

    public string? UbicacionAlmacen { get; set; }

    public virtual LogEjecucion Ejecucion { get; set; } = null!;

    public virtual ICollection<LogInpOfeOtros> LogInpOfeOtros { get; set; } = new List<LogInpOfeOtros>();

    public virtual ICollection<LogInpOfeParametro> LogInpOfeParametro { get; set; } = new List<LogInpOfeParametro>();
}
