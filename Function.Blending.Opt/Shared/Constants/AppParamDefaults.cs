using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Shared.Constants;

public static class AppParamDefaults
{
  public static class Keys
  {
    public const string QualityExecutionFormat = "APP_CAL_CODIGO_FORMAT";
    public const string LogisticExecutionFormat = "APP_LOG_CODIGO_FORMAT";
  }
  public static class Values
  {
    public const string QualityExecutionFormat = "CAL{0:D6}";
    public const string LogisticExecutionFormat = "LOG{0:D6}";
  }
}
