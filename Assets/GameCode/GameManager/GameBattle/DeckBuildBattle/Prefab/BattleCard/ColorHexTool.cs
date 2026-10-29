using UnityEngine;

public static class ColorHexTool
{

    public static string GetColorHexString(this Color color, bool includeAlpha = false, bool withSharpSign = true)
    {
        byte r = (byte)Mathf.Round(color.r * 255);
        byte g = (byte)Mathf.Round(color.g * 255);
        byte b = (byte)Mathf.Round(color.b * 255);
        byte a = (byte)Mathf.Round(color.a * 255);

        string rHex = r.ToString("X2");
        string gHex = g.ToString("X2");
        string bHex = b.ToString("X2");
        string aHex = a.ToString("X2");

        string hexBase = $"{rHex}{gHex}{bHex}";
        if (includeAlpha) hexBase += aHex;
        if (withSharpSign) hexBase = $"#{hexBase}";

        return hexBase;
    }
}

