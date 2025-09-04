using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class LogOutConDistribucion
{
    public Guid Id { get; set; }

    public Guid ContenedorId { get; set; }

    public string Ruma { get; set; } = null!;

    public int Valor { get; set; }

    public virtual LogOutContenedor Contenedor { get; set; } = null!;
}
