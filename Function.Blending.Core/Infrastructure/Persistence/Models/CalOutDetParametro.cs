using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class CalOutDetParametro
{
    public Guid Id { get; set; }

    public Guid DetalleId { get; set; }

    public string CodigoParametro { get; set; } = null!;

    public decimal Valor { get; set; }

    public virtual CalOutDetalle Detalle { get; set; } = null!;
}
