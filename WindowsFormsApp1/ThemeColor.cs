using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class ThemeColor
    {
        public static Color PrimaryColor { get; set; }
        public static Color SecondaryColor { get; set; }

        public static List<string> ColorList = new List<string>()
        {
            "#1B5E20", // Dark Emerald Green — глубокий зелёный
            "#2E7D32", // Forest Green — насыщенный зелёный
            "#004D40", // Teal Dark — тёмная бирюза
            "#00695C", // Deep Teal — насыщенная бирюза
            "#0D47A1", // Dark Blue — тёмный насыщенный синий
            "#1976D2", // Blue — средний насыщенный синий
            "#1A237E", // Indigo Dark — тёмный индиго
            "#283593", // Deep Indigo — насыщенный индиго
            "#4A148C", // Plum Dark — глубокий фиолетовый
            "#6A1B9A", // Purple — насыщенный фиолетовый
            "#311B92", // Deep Purple — глубокий пурпурный
            "#B71C1C", // Crimson Dark — тёмный красный
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
}
