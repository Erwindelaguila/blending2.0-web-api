using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Common.Commands;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Planta.Commands;

public class CreatePlantaCommand : BaseCommand<PlantaDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public int NumeroRuma { get; }
    public bool? Activo { get; }

    [JsonConstructor]
    public CreatePlantaCommand(
        string codigo,
        string nombre,
        string? descripcion = null,
        int numeroRuma = 0,
        bool? activo = null,
        HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        NumeroRuma = numeroRuma;
        Activo = activo ?? true;
    }
}
