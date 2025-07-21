using MediatR;
namespace Function.Blending.Core.Application.Calidad.Commands;

public class DeleteCalidadCommand : IRequest<bool>
{
    public Guid Id { get; }
    public Guid ModificadoPorId { get; }

    public DeleteCalidadCommand(Guid id, Guid modificadoPorId)
    {
        Id = id;
        ModificadoPorId = modificadoPorId;
    }
}
