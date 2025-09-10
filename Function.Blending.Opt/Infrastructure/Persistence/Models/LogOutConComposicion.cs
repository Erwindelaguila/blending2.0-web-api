using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogOutConComposicion
{
    public Guid Id { get; set; }

    public Guid ContenedorId { get; set; }

    public string CodigoParametro { get; set; } = null!;

    public decimal Valor { get; set; }

    public virtual LogOutContenedor Contenedor { get; set; } = null!;
}
