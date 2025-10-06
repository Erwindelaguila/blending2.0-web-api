using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.CalidadParametro.Commands;

public class UpsertCalidadParametroBatchCommand : BaseCommand<int>
{
    public List<CalidadParametroCambio> Cambios { get; set; } = new();

    public UpsertCalidadParametroBatchCommand(
        List<CalidadParametroCambio> cambios)
    {
        Cambios = cambios ?? new List<CalidadParametroCambio>();
    }
}

public class CalidadParametroCambio
{
    public Guid CalidadId { get; set; }
    public Guid ParametroId { get; set; }
    public decimal Valor { get; set; }
}
