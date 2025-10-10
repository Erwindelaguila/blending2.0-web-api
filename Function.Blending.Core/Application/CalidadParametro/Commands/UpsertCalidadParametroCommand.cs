using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.CalidadParametro.Commands;

public class UpsertCalidadParametroCommand : BaseCommand<bool>
{
    public Guid CalidadId { get; set; }
    public Guid ParametroId { get; set; }
    public decimal Valor { get; set; }

    public UpsertCalidadParametroCommand(
        Guid calidadId,
        Guid parametroId,
        decimal valor)
    {
        CalidadId = calidadId;
        ParametroId = parametroId;
        Valor = valor;
    }
}
