using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Common.Commands;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Planta.Commands;

public class UpdatePlantaCommand : BaseCommand<PlantaDTO>
{
    public Guid Id { get; }
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public int NumeroRuma { get; }
    public bool? Activo { get; }

    [JsonConstructor]
    public UpdatePlantaCommand(
        Guid id,
        string codigo,
        string nombre,
        string? descripcion = null,
        int numeroRuma = 0,
        bool? activo = null,
        HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        NumeroRuma = numeroRuma;
        Activo = activo;
    }
}
