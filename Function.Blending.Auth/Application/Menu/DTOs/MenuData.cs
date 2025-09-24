using System.Collections.Generic;

namespace Function.Blending.Auth.Application.Menu.DTOs
{
    public class MenuData
    {
        public Dictionary<string, EnlaceItem>? Enlaces { get; set; }
        public List<string>? PermisosUsuario { get; set; }
        public UserInfo? UserInfo { get; set; }
    }
}