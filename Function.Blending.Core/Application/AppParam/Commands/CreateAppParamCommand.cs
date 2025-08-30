using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.AppParam.Commands;

/// <summary>
/// Command para crear un nuevo parámetro de aplicación
/// Hereda de BaseCommand para mantener el contexto necesario para autenticación
/// </summary>
public class CreateAppParamCommand : BaseCommand<object>
{
    public string Key { get; }
    public string Value { get; }
    public string? Description { get; }
    public string? Category { get; }
    public string? Group { get; }
    public bool IsActive { get; }
    public bool IsInternal { get; }
    public bool IsVisible { get; }
    public bool IsDisableable { get; }
    public bool IsRemovable { get; }

    public CreateAppParamCommand(
        string key,
        string value,
        string? description,
        string? category,
        string? group,
        bool isActive,
        bool isInternal,
        bool isVisible,
        bool isDisableable,
        bool isRemovable,
        object requestContext) : base(requestContext)
    {
        Key = key;
        Value = value;
        Description = description;
        Category = category;
        Group = group;
        IsActive = isActive;
        IsInternal = isInternal;
        IsVisible = isVisible;
        IsDisableable = isDisableable;
        IsRemovable = isRemovable;
    }
}
