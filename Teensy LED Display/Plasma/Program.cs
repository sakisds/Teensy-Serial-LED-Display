using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Teensy_LED_Display_Library;

namespace Plasma {
    class Program {
        static void Main(string[] args) {
            int frames = 5000;
            bool benchmark = true; // Set to false if you want to loop forever

            LedDisplay display = new LedDisplay(32, 16, SerialPort.GetPortNames()[0]);
            display.OpenSerial();

            // Prepare pallete
            LedColor[] palette = new LedColor[360];
            for (int x = 0; x < 360; x++) {
                int r, g, b;
                HsvToRgb(x, 1.0, 1.0, out r, out g, out b);
                palette[x] = new LedColor {
                    R = (byte)r,
                    G = (byte)g,
                    B = (byte)b
                };
            }

            int[,] cls = null;
            int start = Environment.TickCount;
            int i = 0;

            while (i < frames) {
                int w = 32;
                int h = 16;

                if (cls == null) {
                    cls = new int[w, h];
                    for (int x = 0; x < w; x++) {
                        for (int y = 0; y < h; y++) {
                            cls[x, y] = (int)(
                                128.0 + (128.0 * Math.Sin(x / 2.0))
                                + 128.0 + (128.0 * Math.Sin(y / 4.0))
                                + 128.0 +
                                (128.0 * Math.Sin(Math.Sqrt(((x - w / 2.0) * (x - w / 2.0) + (y - h / 2.0) * (y - h / 2.0))) / 2.0))
                                + 128.0 + (128.0 * Math.Sin(Math.Sqrt((x * x + y * y)) / 2.0))
                                ) / 4;
                        }
                    }
                }

                int paletteShift = Convert.ToInt32(Environment.TickCount / 5);
                for (int x = 0; x < w; x++) {
                    for (int y = 0; y < h; y++) {
                        display.DrawPixel(x, y, palette[(cls[x, y] + paletteShift) % 360]);
                    }
                }

                display.RefreshDisplay();
                if (benchmark) i++;
            }

            int elapsed = Environment.TickCount - start;
            double fps = frames/(elapsed/1000.0);
            double bits = (fps*((32*16*3) + 1) * 8)/1000;
            double bandwidth = (bits / 12000) * 100;

            Console.WriteLine("Time elapsed: " + elapsed);
            Console.WriteLine("Frames per second: " + fps);
            Console.WriteLine("kb/s: " + bits + " (" + bandwidth + "% of maximum theoritical serial output)");
            Console.ReadKey();
        }

        static void HsvToRgb(double h, double S, double V, out int r, out int g, out int b) {
            double H = h;
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
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        static int Clamp(int i) {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}
