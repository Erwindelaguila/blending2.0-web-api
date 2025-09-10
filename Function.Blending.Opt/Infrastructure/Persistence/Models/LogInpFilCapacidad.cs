using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogInpFilCapacidad
{
    public Guid Id { get; set; }

    public Guid FiltroId { get; set; }

    public int Cantidad { get; set; }

    public int Capacidad { get; set; }

    public virtual LogInpFiltro Filtro { get; set; } = null!;
}
