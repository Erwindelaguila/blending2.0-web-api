using MediatR;


namespace Function.Blending.Core.Application.Agregado.Commands;

public class DeleteAgregadoCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
