using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogInpDemanda
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public int Posicion { get; set; }

    public string Material { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal CantidadAsignadaVenta { get; set; }

    public string UnidadMedidaVenta { get; set; } = null!;

    public decimal CantidadAsignadaAlmacen { get; set; }

    public string UnidadMedidaAlmacen { get; set; } = null!;

    public decimal Tolerancia { get; set; }

    public virtual LogEjecucion Ejecucion { get; set; } = null!;

    public virtual ICollection<LogInpDemParametro> LogInpDemParametro { get; set; } = new List<LogInpDemParametro>();
}
