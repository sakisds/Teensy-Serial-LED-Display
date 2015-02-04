using System;
using System.IO.Ports;
using System.Threading;
using Teensy_LED_Display_Library;

namespace LibraryTest1 {
    class Program {
        static void Main(string[] args) {
            LedDisplay display = new LedDisplay(32, 16, SerialPort.GetPortNames()[0]);
            display.OpenSerial();

            display.SetBrightness(120);
            display.FillDisplay(LedColor.White);
            display.RefreshDisplay();
            Thread.Sleep(1000000);
            
            Console.WriteLine("Drawing Lines");
            display.DrawLine(0, 0, 5, 10, LedColor.Red);
            display.DrawLine(5, 0, 10, 10, LedColor.Green);
            display.DrawLine(10, 0, 15, 10, LedColor.Blue);
            display.RefreshDisplay();
            Thread.Sleep(1000);

            Console.WriteLine("Drawing circles");
            display.ClearDisplay();
            display.DrawCircle(12, 6, 5, LedColor.Yellow);
            display.RefreshDisplay();
            Thread.Sleep(1000);

            Console.WriteLine("Drawing rectangles");
            display.ClearDisplay();
            display.DrawRectangle(0, 0, 10, 12, LedColor.Orange, LedColor.Cyan);
            display.DrawRectangle(15, 0, 18, 4, LedColor.Purple);
            display.RefreshDisplay();
            Thread.Sleep(1000);

            
            Console.WriteLine("Drawing text");
            String time = DateTime.Now.ToString("HH:mm");
            display.ClearDisplay();
            display.DrawString(0, 0, LedColor.Blue, time, 0, Fonts.Font6X8);
            display.RefreshDisplay();

            Console.ReadKey();
        }
    }
}
