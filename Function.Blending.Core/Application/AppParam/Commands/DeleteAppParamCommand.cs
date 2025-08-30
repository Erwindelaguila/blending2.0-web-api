using MediatR;

namespace Function.Blending.Core.Application.AppParam.Commands;

public class DeleteAppParamCommand : IRequest<object>
{
    public string Key { get; }
    
    public DeleteAppParamCommand(string key)
    {
        Key = key;
    }
}
