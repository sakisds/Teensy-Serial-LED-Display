using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Teensy_LED_Display_Library;

namespace BrightnessTest {
    class Program {
        static void Main(string[] args) {
            LedDisplay display = new LedDisplay(32, 16, SerialPort.GetPortNames()[0]);
            display.OpenSerial();

            display.DrawBitmap(new Bitmap("bitmap.bmp"), 0, 0, 32, 16);
            display.RefreshDisplay();

            for (byte i = 255; i > 0; i -= 5) {
                Console.WriteLine("Brightness: " + i);
                display.SetBrightness(i);
                Thread.Sleep(250);
            }
        }
    }
}
