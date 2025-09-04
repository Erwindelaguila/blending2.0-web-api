using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class CalOutDetOtros
{
    public Guid Id { get; set; }

    public Guid DetalleId { get; set; }

    public string Codigo { get; set; } = null!;

    public string Valor { get; set; } = null!;

    public virtual CalOutDetalle Detalle { get; set; } = null!;
}
