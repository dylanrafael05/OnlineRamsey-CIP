using UnityEngine;

namespace Ramsey.Utilities
{
    public static class Colors 
    {
        public static Color Red => Color.red;
        public static Color Orange => Color.HSVToRGB(26f / 360, 0.73f, 0.96f);
        public static Color Yellow => Color.yellow;
        public static Color Lime => Color.HSVToRGB(108f / 360, 0.73f, 0.96f);
        public static Color Teal => Color.HSVToRGB(169f / 360, 0.73f, 0.96f);
        public static Color Cyan => Color.HSVToRGB(196f / 360, 0.73f, 0.96f);
        public static Color Blue => Color.HSVToRGB(233f / 360, 0.73f, 0.96f);
        public static Color Indigo => Color.HSVToRGB(257f / 360, 0.73f, 0.96f);
        public static Color Purple => Color.HSVToRGB(287f / 360, 0.73f, 0.96f);
        public static Color Pink => Color.magenta;

        public static readonly Color[] Rainbow = 
        {
            Red, Orange, Yellow, Lime, Teal, Cyan, Blue, Indigo, Purple, Pink
        };
    }
}