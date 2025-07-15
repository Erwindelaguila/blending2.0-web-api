namespace Function.Blending.Core.Application.Constants;

public class ApiRoutes
{
    public static class Core
    {
        //rutas de produccion
        public static class Production
        {
            public const string Producto = "core/produccion/producto";
            public const string Calidad = "core/produccion/calidad";
        }
        
        //rutas de planta
        public static class Planta
        {
            public const string Base = "core/planta";
            public const string GetById = "core/planta/detail";
        }
        
        //rutas de parametro
        public static class Parametro
        {
            public const string Base = "core/parametro";
            public const string GetById = "core/parametro/detail";
        }
        
        //rutas de administracion 
        
    }
}