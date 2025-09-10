using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogInpOferta
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public int Posicion { get; set; }

    public string Material { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int CantidadAsignadaVenta { get; set; }

    public int UnidadMedidaVenta { get; set; }

    public int CantidadAsignadaAlmacen { get; set; }

    public int UnidadMedidaAlmacen { get; set; }

    public decimal Tolerancia { get; set; }

    public virtual LogEjecucion Ejecucion { get; set; } = null!;

    public virtual ICollection<LogInpOfeParametro> LogInpOfeParametro { get; set; } = new List<LogInpOfeParametro>();
}
