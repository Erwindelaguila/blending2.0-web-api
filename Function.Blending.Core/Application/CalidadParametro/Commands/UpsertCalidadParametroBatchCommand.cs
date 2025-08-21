using MediatR;

namespace Function.Blending.Core.Application.CalidadParametro.Commands;

public class UpsertCalidadParametroBatchCommand : IRequest<int>
{
    public List<CalidadParametroCambio> Cambios { get; set; } = new();
    public Guid ModificadoPorId { get; set; }
}

public class CalidadParametroCambio
{
    public Guid CalidadId { get; set; }
    public Guid ParametroId { get; set; }
    public decimal Valor { get; set; }
}
