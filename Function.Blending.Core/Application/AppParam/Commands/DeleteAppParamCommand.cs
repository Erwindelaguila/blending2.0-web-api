using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.AppParam.Commands;

public class DeleteAppParamCommand : BaseCommand<bool>
{
    public string Key { get; }

    public DeleteAppParamCommand(string key, object requestContext) : base(requestContext)
    {
        Key = key;
    }
}
