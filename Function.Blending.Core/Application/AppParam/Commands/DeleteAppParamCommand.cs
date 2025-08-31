using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.AppParam.Commands;

/// <summary>
/// Command para eliminar un parámetro de aplicación
/// Hereda de BaseCommand para mantener el contexto necesario para autenticación
/// </summary>
public class DeleteAppParamCommand : BaseCommand<bool>
{
    public string Key { get; }
    
    public DeleteAppParamCommand(string key, object requestContext) : base(requestContext)
    {
        Key = key;
    }
}
