using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class LogInpOfeParametro
{
    public Guid Id { get; set; }

    public Guid OfertaId { get; set; }

    public string CodigoParametro { get; set; } = null!;

    public decimal Valor { get; set; }

    public virtual LogInpOferta Oferta { get; set; } = null!;
}
