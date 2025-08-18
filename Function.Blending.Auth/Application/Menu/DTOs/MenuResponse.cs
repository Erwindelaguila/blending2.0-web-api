using System.Net;

namespace Function.Blending.Auth.Application.Menu.DTOs
{
    public class MenuResponse
    {
        public bool Success { get; }
        public MenuData? Data { get; }
        public string? Message { get; }
        public int StatusCode { get; }

        private MenuResponse(bool success, MenuData? data, string? message, int statusCode)
        {
            Success = success;
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }

        public static MenuResponse CreateSuccess(MenuData data, string? message = null)
        {
            return new MenuResponse(
                success: true,
                data: data,
                message: message,
                statusCode: (int)HttpStatusCode.OK
            );
        }

        public static MenuResponse CreateFailure(string message, HttpStatusCode statusCode)
        {
            return new MenuResponse(
                success: false,
                data: null,
                message: message,
                statusCode: (int)statusCode
            );
        }
    }

    public class MenuData
    {
        public Dictionary<string, EnlaceItem>? Enlaces { get; set; }
        public List<string>? PermisosUsuario { get; set; }
        public UserInfo? UserInfo { get; set; }
    }
}
