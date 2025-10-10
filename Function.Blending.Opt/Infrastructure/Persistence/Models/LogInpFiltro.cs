using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogInpFiltro
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public decimal PesoContenedor { get; set; }

    public bool ActualizarCapacidad { get; set; }

    public string? Parametros { get; set; }

    public bool? HabilitarDivision { get; set; }

    public string? Division { get; set; }

    public decimal? TiempoEspera { get; set; }

    public virtual LogEjecucion Ejecucion { get; set; } = null!;

    public virtual ICollection<LogInpFilCapacidad> LogInpFilCapacidad { get; set; } = new List<LogInpFilCapacidad>();

    public virtual ICollection<LogInpFilDivision> LogInpFilDivision { get; set; } = new List<LogInpFilDivision>();

    public virtual ICollection<LogInpFilEmparejamiento> LogInpFilEmparejamiento { get; set; } = new List<LogInpFilEmparejamiento>();
}
