namespace Function.Blending.Upload.Functions.Support.Routing;

public class FunctionRoutes
{
    public const string ApiBase = "/upload/";

    public static class Health
    {
        public const string Check = $"{ApiBase}health";
    }
    
}