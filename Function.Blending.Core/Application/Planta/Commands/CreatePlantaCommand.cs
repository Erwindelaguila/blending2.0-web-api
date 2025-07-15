using Function.Blending.Core.Application.Planta.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Commands;

public class CreatePlantaCommand : IRequest<PlantaDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public int NumeroRuma { get; }
    public bool? Activo { get; }
    public Guid CreadoPorId { get; }

    public CreatePlantaCommand(
        string codigo,
        string nombre,
        string? descripcion,
        int numeroRuma,
        bool? activo,
        Guid creadoPorId)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        NumeroRuma = numeroRuma;
        Activo = activo;
        CreadoPorId = creadoPorId;
    }
}
