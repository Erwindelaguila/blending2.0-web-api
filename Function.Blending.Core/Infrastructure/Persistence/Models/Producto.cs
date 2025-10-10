using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class Producto
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public Guid CalidadId { get; set; }

    public Guid TipoProduccionId { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public Guid? EliminadoPorId { get; set; }

    public DateTime? EliminadoEl { get; set; }

    public bool Eliminado { get; set; }

    public virtual Calidad Calidad { get; set; } = null!;

    public virtual TipoProduccion TipoProduccion { get; set; } = null!;
}
