using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class TipoProduccion
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public Guid LineaProduccionId { get; set; }

    public Guid AgregadoId { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public Guid? EliminadoPorId { get; set; }

    public DateTime? EliminadoEl { get; set; }

    public bool Eliminado { get; set; }

    public virtual Agregado Agregado { get; set; } = null!;

    public virtual LineaProduccion LineaProduccion { get; set; } = null!;

    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();
}
