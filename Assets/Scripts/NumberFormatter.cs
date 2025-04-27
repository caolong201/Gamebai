
using UnityEngine;

public static class NumberFormatter
{
    public static string FormatCoins(this int amount)
    {
        if (amount < 1000)
        {
            return amount.ToString();
        }
        else if (amount < 1000000)
        {
            // Format as K (thousands)
            float thousands = amount / 1000f;
            return FormatDecimal(thousands) + "K";
        }
        else
        {
            // Format as M (millions)
            float millions = amount / 1000000f;
            return FormatDecimal(millions) + "M";
        }
    }
    
    private static string FormatDecimal(float value)
    {
        // If it's a whole number, show without decimal
        if (value == Mathf.Floor(value))
        {
            return value.ToString("0");
        }
        
        // Otherwise show 1 decimal place
        return value.ToString("0.0").TrimEnd('0').TrimEnd('.');
    }
}
