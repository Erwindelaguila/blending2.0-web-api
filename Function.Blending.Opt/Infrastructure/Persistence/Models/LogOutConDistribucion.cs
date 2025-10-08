using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogOutConDistribucion
{
    public Guid Id { get; set; }

    public Guid ContenedorId { get; set; }

    public string Ruma { get; set; } = null!;

    public decimal Valor { get; set; }

    public virtual LogOutContenedor Contenedor { get; set; } = null!;
}
