using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teensy_LED_Display_Library {
    abstract class SerialCommands {
        public const byte Refresh = 0xFF;
        public const byte SetBrightness = 0xFE;
        public const byte ReadFrame = 0xFD;
    }
}
