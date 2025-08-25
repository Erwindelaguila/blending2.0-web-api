namespace Function.Blending.Core.Application.Constants;

public class ApiRoutes
{
    public static class Core
    {
        // Rutas de producción (todas las entidades principales agrupadas)
        public static class Production
        {
            // Producto
            public const string ProductoBase = "core/produccion/producto";
            public const string ProductoGetById = "core/produccion/producto/detail";

            // Calidad
            public const string CalidadBase = "core/produccion/calidad";
            public const string CalidadGetById = "core/produccion/calidad/detail";

            // Agregado
            public const string AgregadoBase = "core/produccion/agregado";
            public const string AgregadoGetById = "core/produccion/agregado/detail";

            // Línea de Producción
            public const string LineaProduccionBase = "core/produccion/lineaproduccion";
            public const string LineaProduccionGetById = "core/produccion/lineaproduccion/detail";

            // Tipo de Producción
            public const string TipoProduccionBase = "core/produccion/tipoproduccion";
            public const string TipoProduccionGetById = "core/produccion/tipoproduccion/detail";

            // Calidad-Parametro
            public const string CalidadParametroMatriz = "core/produccion/calidad-parametros/matriz";
            public const string CalidadParametroUpsert = "core/produccion/calidad-parametros/upsert";
        }

        // Rutas de planta
        public static class Planta
        {
            public const string Base = "core/planta";
            public const string GetById = "core/planta/detail";
        }

        // Rutas de parámetro
        public static class Parametro
        {
            public const string Base = "core/parametro";
            public const string GetById = "core/parametro/detail";
        }

        // Rutas de configuraciones
        public static class Configuraciones
        {
            public const string CalidadParametroMatriz = "core/configuraciones/calidadparametro/matriz";
            public const string CalidadParametroUpsert = "core/configuraciones/calidadparametro/upsert";
        }

        // Rutas de AppParam
        public static class AppParam
        {
            public const string Base = "core/appparam";
            public const string GetByKey = "core/appparam/detail";
            public const string GetByCategory = "core/appparam/category";
            public const string GetByGroup = "core/appparam/group";
        }

        // Rutas de usuario y autenticación
        public static class User
        {
            public const string Menu = "core/user/menu";
        }
    }
}