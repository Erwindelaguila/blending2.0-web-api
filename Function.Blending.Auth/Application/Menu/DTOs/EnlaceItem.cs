using System.Text.Json.Serialization;

namespace Function.Blending.Auth.Application.Menu.DTOs
{
    
    public class EnlaceItem
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
        
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
        
        [JsonPropertyName("color")]
        public string? Color { get; set; }
        
        [JsonPropertyName("grupo")]
        public string? Grupo { get; set; }
    }
}
