using MediatR;

namespace Function.Blending.Core.Application.CalidadParametro.Commands;

public class UpsertCalidadParametroCommand : IRequest<bool>
{
    public Guid CalidadId { get; set; }
    public Guid ParametroId { get; set; }
    public decimal Valor { get; set; }
    public Guid ModificadoPorId { get; set; }
}
