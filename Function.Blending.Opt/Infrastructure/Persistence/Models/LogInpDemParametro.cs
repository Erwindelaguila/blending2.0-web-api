using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogInpDemParametro
{
    public Guid Id { get; set; }

    public Guid DemandaId { get; set; }

    public string CodigoParametro { get; set; } = null!;

    public decimal Valor { get; set; }

    public virtual LogInpDemanda Demanda { get; set; } = null!;
}
