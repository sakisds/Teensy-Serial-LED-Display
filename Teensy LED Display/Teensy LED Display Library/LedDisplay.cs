using System;
using System.IO.Ports;

namespace Teensy_LED_Display_Library {

    /// <summary>
    /// Represents a LedDisplay
    /// </summary>
    public partial class LedDisplay {
        /// <summary>
        /// Display width (in pixels)
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Display height (in pixels)
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Display size (total pixels)
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Autoreconnect in case serial port fails
        /// </summary>
        public bool AutoReconnect { get; set; }

        /// <summary>
        /// Draws a clock above every frame
        /// </summary>
        public bool ShowClock { get; set; }

        // The serial port
        private readonly SerialPort _serialPort;

        // Display buffers
        private LedColor[] _backBuffer, _frontBuffer;

        /// <summary>
        /// Creates a new LED display
        /// </summary>
        /// <param name="width">Width of the display (in pixels)</param>
        /// <param name="height">Height of the display (in pixels)</param>
        /// <param name="comPort">COM Port</param>
        public LedDisplay(int width, int height, string comPort) {
            // Store size
            Width = width;
            Height = height;
            Size = width * height;

            // Create serial port
            _serialPort = new SerialPort() {
                PortName = comPort,
                BaudRate = 12000000, // Full USB speed
                DtrEnable = true,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };

            // Create the buffers and fill with 0s
            _backBuffer = new LedColor[Size];
            _frontBuffer = new LedColor[Size];
            for (int i = 0; i < Size; i++) {
                _backBuffer[i] = new LedColor { R = 0, G = 0, B = 0 };
                _frontBuffer[i] = new LedColor { R = 0, G = 0, B = 0 };
            }

            // Some defaults
            AutoReconnect = true;
            ShowClock = false;
        }

        /// <summary>
        /// Opens the serial port if it's closed
        /// </summary>
        public void OpenSerial() {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        /// <summary>
        /// Closes the serial port if it's open
        /// </summary>
        public void CloseSerial() {
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        /// <summary>
        /// Refreshes the display.
        /// This will send over to the Teensy the frame data.
        /// 
        /// If autoreconnect is enabled and the connection fails
        /// this will attempt to re-open the serial port but it
        /// won't refresh the display on this cycle.
        /// </summary>
        public void RefreshDisplay() {
            if (_serialPort.IsOpen) {
                // Draw clock if required
                if (ShowClock) {
                    String time = DateTime.Now.ToString("HH:mm");
                    DrawString(1, 4, LedColor.White, time, 0, Fonts.Font6X8);
                }

                // Update the display
                byte[] data = new byte[1 + (Size * 3)]; // 1 bytes for command
                _frontBuffer.CopyTo(_backBuffer, 0);

                int i = 1;
                data[0] = SerialCommands.ReadFrame;
                foreach (LedColor c in _backBuffer) {
                    data[i] = c.R;
                    data[i + 1] = c.G;
                    data[i + 2] = c.B;

                    i += 3;
                }

                _serialPort.Write(data, 0, data.Length);

            } else if (AutoReconnect) {
                _serialPort.Open();
            }
        }

        /// <summary>
        /// Sets the display brightness to the given value
        /// </summary>
        /// <param name="value">Brightness (0-255)</param>
        public void SetBrightness(byte value) {
            if (!_serialPort.IsOpen) return;

            byte[] data = new byte[2];
            data[0] = SerialCommands.SetBrightness;
            data[1] = value;

            _serialPort.Write(data, 0, 2);
        }
    }
}
