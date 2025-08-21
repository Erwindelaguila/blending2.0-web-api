namespace Function.Blending.Core.Application.Constants;

public static class FunctionNames
{
    public static class Calidad
    {
        public const string Create = "CreateCalidad";
        public const string Update = "UpdateCalidad";
        public const string Delete = "DeleteCalidad";
        public const string GetAll = "GetAllCalidades";
        public const string GetById = "GetCalidadById";
        public const string GetAllWithoutPagination = "GetAllCalidadesWithoutPagination";
    }

    public static class Producto
    {
        public const string Create = "CreateProducto";
        public const string Update = "UpdateProducto";
        public const string Delete = "DeleteProducto";
        public const string GetAll = "GetAllProductos";
        public const string GetById = "GetProductoById";
        public const string GetAllWithoutPagination = "GetAllProductosWithoutPagination";
    }

    public static class Planta
    {
        public const string Create = "CreatePlanta";
        public const string Update = "UpdatePlanta";
        public const string Delete = "DeletePlanta";
        public const string GetAll = "GetAllPlantas";
        public const string GetById = "GetPlantaById";
        public const string GetAllWithoutPagination = "GetAllPlantasWithoutPagination";
    }

    public static class Parametro
    {
        public const string Create = "CreateParametro";
        public const string Update = "UpdateParametro";
        public const string Delete = "DeleteParametro";
        public const string GetAll = "GetAllParametros";
        public const string GetById = "GetParametroById";
        public const string GetAllWithoutPagination = "GetAllParametrosWithoutPagination";
    }

    public static class TipoProduccion
    {
        public const string Create = "CreateTipoProduccion";
        public const string Update = "UpdateTipoProduccion";
        public const string Delete = "DeleteTipoProduccion";
        public const string GetAll = "GetAllTipoProduccion";
        public const string GetById = "GetTipoProduccionById";
        public const string GetAllWithoutPagination = "GetAllTipoProduccionWithoutPagination";
    }

    public static class Agregado
    {
        public const string Create = "CreateAgregado";
        public const string Update = "UpdateAgregado";
        public const string Delete = "DeleteAgregado";
        public const string GetAll = "GetAllAgregados";
        public const string GetById = "GetAgregadoById";
        public const string GetAllWithoutPagination = "GetAllAgregadosWithoutPagination";
    }
    public static class LineaProduccion
    {
        public const string Create = "CreateLineaProduccion";
        public const string Update = "UpdateLineaProduccion";
        public const string Delete = "DeleteLineaProduccion";
        public const string GetAll = "GetAllLineasProduccion";
        public const string GetById = "GetLineaProduccionById";
        public const string GetAllWithoutPagination = "GetAllLineasProduccionWithoutPagination";
    }

    public static class CalidadParametro
    {
        public const string GetMatriz = "GetCalidadParametroMatriz";
        public const string Upsert = "UpsertCalidadParametro";
    }

    public static class User
    {
        public const string GetMenu = "GetUserMenu";
    }
    
    // Agrega otras funciones según tu dominio...
}