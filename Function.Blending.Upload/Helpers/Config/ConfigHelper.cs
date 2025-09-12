namespace Function.Blending.Upload.Helpers;

public class ConfigHelper
{
    public static TSpecificConfig CastConfig<TSpecificConfig>(object config) where TSpecificConfig : class
    {
        var casted = config as TSpecificConfig;
        if (casted == null)
            throw new InvalidOperationException($"El config no es del tipo esperado: {typeof(TSpecificConfig).Name}");
        return casted;
    }
    
    public static int ColumnLetterToNumber(string columnLetter)
    {
        int sum = 0;
        foreach (char c in columnLetter)
        {
            sum *= 26;
            sum += (c - 'A' + 1);
        }
        return sum;
    }
    
    public static string ColumnNumberToLetter(int columnNumber)
    {
        string result = "";
        while (columnNumber > 0)
        {
            columnNumber--;
            result = (char)('A' + (columnNumber % 26)) + result;
            columnNumber /= 26;
        }
        return result;
    }
    public static double TryToDouble(object input, double defaultValue = 0.0)
    {
        if (double.TryParse(input?.ToString(), out double result))
        {
            return result;
        }
        return defaultValue;
    }
}