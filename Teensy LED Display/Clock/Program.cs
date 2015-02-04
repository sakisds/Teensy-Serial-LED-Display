using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Teensy_LED_Display_Library;

namespace Clock {
    class Program {
        static void Main(string[] args) {
            LedDisplay display = new LedDisplay(32, 16, SerialPort.GetPortNames()[0]);
            display.OpenSerial();
            display.SetBrightness(175);

            while (true) {
                display.ClearDisplay();

                String time = DateTime.Now.ToString("HH:mm");
                display.DrawString(0, 4, LedColor.Red, time, 0, Fonts.Font6X8);

                display.RefreshDisplay();
                Thread.Sleep(2000);
            }
        }
    }
}
