using Function.Blending.Core.Application.Planta.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Commands;

public class UpdatePlantaCommand : IRequest<object>
{
    public Guid Id { get; }
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public int NumeroRuma { get; }
    public bool? Activo { get; }
    public Guid ModificadoPorId { get; }

    public UpdatePlantaCommand(
        Guid id,
        string codigo,
        string nombre,
        string? descripcion,
        int numeroRuma,
        bool? activo,
        Guid modificadoPorId)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        NumeroRuma = numeroRuma;
        Activo = activo;
        ModificadoPorId = modificadoPorId;
    }
}
