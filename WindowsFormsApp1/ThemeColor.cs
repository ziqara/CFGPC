using System.Collections.Generic;
using System.Drawing;
using System;

public static class ThemeColor
{
    public static Color PrimaryColor { get; set; }
    public static Color SecondaryColor { get; set; }

    public static event Action ThemeChanged;

    public static void SetTheme(Color primary, Color secondary)
    {
        PrimaryColor = primary;
        SecondaryColor = secondary;
        ThemeChanged?.Invoke();
    }

    public static List<string> ColorList = new List<string>()
    {
        "#1B5E20", "#004D40", "#0D47A1", "#4A148C", "#1A237E",
        "#880E4F", "#B71C1C", "#3E2723"
    };

    public static Color ChangeColorBrightness(Color color, double correctionFactor)
    {
        double red = color.R;
        double green = color.G;
        double blue = color.B;

        if (correctionFactor < 0)
        {
            correctionFactor = 1 + correctionFactor;
            red *= correctionFactor;
            green *= correctionFactor;
            blue *= correctionFactor;
        }
        else
        {
            red = (255 - red) * correctionFactor + red;
            green = (255 - green) * correctionFactor + green;
            blue = (255 - blue) * correctionFactor + blue;
        }

        return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
    }
}
