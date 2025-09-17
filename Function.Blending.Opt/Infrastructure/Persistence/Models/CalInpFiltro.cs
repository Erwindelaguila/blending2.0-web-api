using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class CalInpFiltro
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public string CentroUbicacion { get; set; } = null!;

    public string CentroProduccion { get; set; } = null!;

    public string UbicacionAlmacen { get; set; } = null!;

    public bool MezclarTipoProduccion { get; set; }

    public string TipoProduccion { get; set; } = null!;

    public string? BorrarCalidades { get; set; }

    public bool QuitarRumasPH { get; set; }

    public string? AgregarRumasSerie { get; set; }

    public bool ConsiderarCadmio { get; set; }

    public DateTime? FechaCorte { get; set; }

    public int? NumeroRuma { get; set; }

    public int? DivisionRuma { get; set; }

    public decimal ValorCadmioAlto { get; set; }

    public virtual CalEjecucion Ejecucion { get; set; } = null!;
}
