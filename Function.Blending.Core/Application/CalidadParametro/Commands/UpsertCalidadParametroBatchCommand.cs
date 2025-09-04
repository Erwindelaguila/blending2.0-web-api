using Function.Blending.Core.Application.Common.Commands;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.CalidadParametro.Commands;

public class UpsertCalidadParametroBatchCommand : BaseCommand<int>
{
    public List<CalidadParametroCambio> Cambios { get; set; } = new();

    public UpsertCalidadParametroBatchCommand(
        List<CalidadParametroCambio> cambios,
        HttpRequestData requestContext) : base(requestContext)
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
