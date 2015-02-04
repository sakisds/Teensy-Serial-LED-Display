using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teensy_LED_Display_Library {
    /// <summary>
    /// 24-bit color struct
    /// This format is used by the LED display
    /// </summary>
    public struct LedColor {
        public byte R, G, B;

        /// <summary>
        /// Create a color using a .NET color
        /// </summary>
        /// <param name="color"></param>
        public LedColor(Color color) {
            R = color.R;
            G = color.G;
            B = color.B;
        }

        /// <summary>
        /// Create a color from HSV
        /// </summary>
        /// <param name="h">Hue (0-360)</param>
        /// <param name="S">Saturation (0.0-1.0)</param>
        /// <param name="V">Value (0.0-1.0)</param>
        public LedColor(double H, double S, double V) {
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };

            double R, G, B;

            if (V <= 0) { R = G = B = 0; } else if (S <= 0) {
                R = G = B = V;
            } else {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i) {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            this.R = Clamp(R * 255.0);
            this.G = Clamp(G * 255.0);
            this.B = Clamp(B * 255.0);
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        private static byte Clamp(double i) {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return (byte) i;
        }

        #region Equality operators
        public bool Equals(LedColor other) {
            return R == other.R && G == other.G && B == other.B;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LedColor && Equals((LedColor) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = R.GetHashCode();
                hashCode = (hashCode*397) ^ G.GetHashCode();
                hashCode = (hashCode*397) ^ B.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(LedColor c1, LedColor c2) {
            return c1.Equals(c2);
        }

        public static bool operator !=(LedColor c1, LedColor c2) {
            return !c1.Equals(c2);
        }
        #endregion
        #region Some colors

        public static LedColor White    = new LedColor {R = 255,    G = 255,    B = 255};
        public static LedColor Black    = new LedColor { R = 0,     G = 0,      B = 0   };

        public static LedColor Red      = new LedColor { R = 255,   G = 0,      B = 0   };
        public static LedColor Green    = new LedColor { R = 0,     G = 255,    B = 0   };
        public static LedColor Blue     = new LedColor { R = 0,     G = 0,      B = 255 };

        public static LedColor Cyan     = new LedColor { R = 0,     G = 255,    B = 255 };
        public static LedColor Purple   = new LedColor { R = 255,   G = 0,      B = 255 };
        public static LedColor Yellow   = new LedColor { R = 255,   G = 255,    B = 0   };

        public static LedColor Orange   = new LedColor { R = 255,   G = 100,    B = 0   };
        
        #endregion
    }
}
