using Function.Blending.Core.Application.AuxRow.DTOs;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IAuxRowRepository
{
    Task<List<StatusRowDTO>> GetStatusQualityAsync(Guid id);
}