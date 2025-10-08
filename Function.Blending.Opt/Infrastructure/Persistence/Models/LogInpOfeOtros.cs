using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogInpOfeOtros
{
    public Guid Id { get; set; }

    public Guid OfertaId { get; set; }

    public string Codigo { get; set; } = null!;

    public string Valor { get; set; } = null!;

    public virtual LogInpOferta Oferta { get; set; } = null!;
}
