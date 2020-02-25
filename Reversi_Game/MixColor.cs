using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Reversi_Game
{
    static class MixColor
    {
        public static Color Lerp(this Color color, Color diffrentColor, double weight)
        {
            byte r = (byte)(weight * color.R + (1 - weight) * diffrentColor.R);
            byte g = (byte)(weight * color.G + (1 - weight) * diffrentColor.G);
            byte b = (byte)(weight * color.B + (1 - weight) * diffrentColor.B);
            return Color.FromRgb(r, g, b);
        }

        public static SolidColorBrush Lerp(this SolidColorBrush brush, SolidColorBrush diffrentBrush, double weight)
        {
            return new SolidColorBrush(Lerp(brush.Color, diffrentBrush.Color, weight));
        }
    }
}
