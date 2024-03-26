using System;

public static class EnumParser
{
    public static bool TryParse<TEnum>(string value, out TEnum result, out int parsedValue) where TEnum : struct, Enum
    {
        if (Enum.TryParse(value, true, out result))
        {
            parsedValue = Convert.ToInt32(result);
            return true;
        }
        else
        {
            result = default;
            parsedValue = -1;
            return false;
        }
    }
    
    public static bool TryParse<TEnum>(string value, out string result, out int parsedValue) where TEnum : struct, Enum
    {
        if (Enum.TryParse(value, true, out TEnum enumResult))
        {
            result = enumResult.ToString();
            parsedValue = Convert.ToInt32(enumResult);
            return true;
        }
        else
        {
            result = string.Empty;
            parsedValue = -1;
            return false;
        }
    }
}