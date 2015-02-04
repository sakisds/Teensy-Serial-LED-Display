# Teensy Serial LED Display

### Overview
This is a library about controlling a LED display connected to a Teensy 3.1 using Serial.
The library is mostly feature complete, it includes a couple of drawing functions as well
for convinience.

There are a couple of demos included:
* Clock: Displays a simple digital clock on the display
* LibraryTest1: Tests drawing functions (lines, text, etc)
* Plasma: Colors and rainbows!
* BrightnessTest: Circles around all brightness levels (put a bitmap.bmp in the folder).

### How to use
Check the LedDisplay object. The drawing functions are in a seperate file (LedDisplayDrawing.cs)
for readability. Remember to call RefreshDisplay() every time you are finished drawing to the buffer
otherwise the display won't refresh.

### Hardware
On the hardware side we have a Teensy 3.1 board, a [32x16 LED Matrix](https://www.sparkfun.com/products/12583)
and the [SmartMatrix shield](http://www.pixelmatix.com/).

### Licensing
The library is licensed under the BSD license (check License.txt).