using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class CalInpParametro
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public Guid CalidadId { get; set; }

    public Guid ParametroId { get; set; }

    public decimal Valor { get; set; }

    public virtual Calidad Calidad { get; set; } = null!;

    public virtual CalEjecucion Ejecucion { get; set; } = null!;

    public virtual Parametro Parametro { get; set; } = null!;
}
