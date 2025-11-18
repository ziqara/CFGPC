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
        public static List<string> ColorList = new List<string>()
        {
            "#004D40",  // темный бирюзовый (добавляет свежести, сочетается с синим)
            "#BF360C",  // темный красно-оранжевый (комплементарный акцент)
            "#4A148C",  // темный фиолетовый (сочетается с синим через холодные тона)
            "#880E4F",  // темный малиновый (добавляет глубины)
            "#B71C1C",  // темный красный (контрастный акцент)
            "#3E2723",  // темный пурпурный (темный, нейтральный)
            "#2E7D32",  // темный зеленый (комплементарный к синему)
            "#880E4F",  // темный розовый (мягкий акцент)
            "#3E2723"   // темный темно-красный (глубокий, сочетается)
        };

        public static Color ChangeColorBrightness(Color color, double correctionFactor)
        {
            double red = color.R;  // Исправлено: R вместо A
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

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue); // Исправлено: FromArgb
        }
    }
}
