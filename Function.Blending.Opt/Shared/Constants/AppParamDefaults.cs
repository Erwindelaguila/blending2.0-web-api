namespace Function.Blending.Opt.Shared.Constants;

public static class AppParamDefaults
{
  public static class Keys
  {
    public const string QualityExecutionFormat = "APP_CAL_CODIGO_FORMAT";
    public const string LogisticExecutionFormat = "APP_LOG_CODIGO_FORMAT";
    public const string LogisticaTiempoEspera = "APP_LOG_TIEMPO_ESPERA_DEFAULT";
    public const string LogisticaValorDivision = "APP_LOG_VALOR_DIVISION_DEFAULT";
  }
  public static class Values
  {
    public const string QualityExecutionFormat = "CAL{0:D6}";
    public const string LogisticExecutionFormat = "LOG{0:D6}";
    public const decimal LogisticaTiempoEspera = 300;
    public const string LogisticaValorDivision = "40";
  }
}
