using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class Calidad
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? CodigoMaterial { get; set; }

    public string? Descripcion { get; set; }

    public bool NoConforme { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual ICollection<CalidadParametro> CalidadParametros { get; set; } = new List<CalidadParametro>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}