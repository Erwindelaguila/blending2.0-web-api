namespace Function.Blending.Upload.Functions.Support.Routing;

public class FunctionRoutes
{
    public const string ApiBase = "upload";

    public static class Health
    {
        public const string Check = $"{ApiBase}/health";
    }

    public static class SapStock
    {
        public const string get = $"{ApiBase}/get-sap-stock";
    }

    public static class Cadmio
    {
        public const string get = $"{ApiBase}/get-cadmio-rumas";
    }

    public static class Logistic
    {
        public const string upload = $"{ApiBase}/upload-excel-logistics";
        public const string write = $"{ApiBase}/write-excel-logistic";

    }
    
    
    public static class Quality
    {
        public const string upload = $"{ApiBase}/upload-excel-quality";
        public const string write = $"{ApiBase}/write-excel-quality";

    }

}