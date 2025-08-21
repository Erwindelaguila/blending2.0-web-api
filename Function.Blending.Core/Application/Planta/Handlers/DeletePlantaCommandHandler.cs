using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Planta.Commands;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class DeletePlantaCommandHandler : IRequestHandler<DeletePlantaCommand, BaseResponse<object>>
{
    private readonly IPlantaRepository _plantaRepository;

    public DeletePlantaCommandHandler(IPlantaRepository plantaRepository)
    {
        _plantaRepository = plantaRepository;
    }

    public async Task<BaseResponse<object>> Handle(DeletePlantaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var planta = await _plantaRepository.GetByIdAsync(request.Id);
            
            if (planta == null)
                return BaseResponse<object>.Fail("Planta no encontrada", null, 404);

            await _plantaRepository.DeleteAsync(request.Id, request.EliminadoPorId);
            
            return BaseResponse<object>.Success(new { }, "Planta eliminada correctamente");
        }
        catch (Exception ex)
        {
            return BaseResponse<object>.Fail($"Error al eliminar la planta: {ex.Message}", null, 500);
        }
    }
}
