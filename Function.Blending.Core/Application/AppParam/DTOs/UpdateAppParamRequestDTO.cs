using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.AppParam.DTOs;


public class UpdateAppParamRequestDTO
{
    public string? Key { get; set; } 
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Group { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsInternal { get; set; }
    public bool? IsVisible { get; set; }
    public bool? IsDisableable { get; set; }
    public bool? IsRemovable { get; set; }
}
