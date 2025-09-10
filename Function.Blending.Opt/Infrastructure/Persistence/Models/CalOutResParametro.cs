using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class CalOutResParametro
{
    public Guid Id { get; set; }

    public Guid ResumenId { get; set; }

    public string CodigoParametro { get; set; } = null!;

    public decimal Valor { get; set; }

    public virtual CalOutResumen Resumen { get; set; } = null!;
}
