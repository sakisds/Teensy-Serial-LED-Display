/*
 * This file includes all the drawing methods for LedDisplay.
 * Some of these are based on work of others as indicated in their
 * description and in License.txt.
 * 
 * Generally, this is a mix of the SmartMatrix drawing functions
 * and the AdafruitGFX library.
 * 
 * An interesting sideproject would be replacing this with serial
 * commands that call the corresponding methods on the Teensy but
 * I believe that would probably be slower.
 */

using System;
using System.Drawing;

namespace Teensy_LED_Display_Library {
    public partial class LedDisplay {
        /// <summary>
        /// Sets one pixel to the given color.
        /// </summary>
        /// <param name="x">X coords</param>
        /// <param name="y">Y coords</param>
        /// <param name="color">24-bit color</param>
        public void DrawPixel(int x, int y, LedColor color) {
            if (x < Width && y < Height && x >= 0 && y >= 0) // Standard display size checks
                _frontBuffer[(y * Width) + x] = color;
        }

        /// <summary>
        /// Sets one pixel to given color.
        /// </summary>
        /// <param name="x">X coords</param>
        /// <param name="y">Y coords</param>
        /// <param name="color">.NET color</param>
        public void DrawPixel(int x, int y, Color color) {
            DrawPixel(x, y, new LedColor(color));
        }

        /// <summary>
        /// Draws a line from (x0, y0) to (x1, y1) using Bresenham's
        /// algorithm.
        /// </summary>
        /// <remarks>
        /// Based on code from the AdafruitGFX library
        /// Written by Limor Fried/Ladyada for Adafruit Industries
        /// 
        /// Check License.txt for more info
        /// </remarks>
        /// <param name="x0">Start X coords</param>
        /// <param name="y0">Start Y coords</param>
        /// <param name="x1">End X coords</param>
        /// <param name="y1">End Y coords</param>
        /// <param name="color">24-bit Color</param>
        public void DrawLine(int x0, int y0, int x1, int y1, LedColor color) {
            // Check for easy lines
            if (x0 == x1) {
                DrawFastVLine(x0, y0, Math.Abs(y0 - y1), color);
                return;
            }
            if (y0 == y1) {
                DrawFastHLine(x0, y0, Math.Abs(x0 - x1), color);
                return;
            }

            // If the lines are not vertical or horizontal, draw them the hard way
            // Check steep
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) {
                // Swap x0 and y0
                x0 = x0 + y0;
                y0 = x0 - y0;
                x0 = x0 - y0;

                // Swap x1 and y1
                x1 = x1 + y1;
                y1 = x1 - y1;
                x1 = x1 - y1;
            }

            if (x0 > x1) {
                // Swap x0 and x1
                x0 = x0 + x1;
                x1 = x0 - x1;
                x0 = x0 - x1;

                // Swap y0 and y1
                y0 = y0 + y1;
                y1 = y0 - y1;
                y0 = y0 - y1;
            }

            int dx, dy;
            dx = x1 - x0;
            dy = Math.Abs(y1 - y0);

            int error = dx / 2;
            int yStep;

            if (y0 < y1) {
                yStep = 1;
            } else {
                yStep = -1;
            }

            for (; x0 <= x1; x0++) {
                if (steep) {
                    DrawPixel(y0, x0, color);
                } else {
                    DrawPixel(x0, y0, color);
                }
                error -= dy;
                if (error < 0) {
                    y0 += (byte) yStep;
                    error += dx;
                }
            }
        }

        /// <summary>
        /// Draws a horizontal line. Since horizontal lines are easy this avoids running
        /// Bresenham's algorithm.
        /// </summary>
        /// <param name="x0">Start X coords</param>
        /// <param name="y0">Start Y coords</param>
        /// <param name="length">Length of line</param>
        /// <param name="color">Color of line</param>
        private void DrawFastHLine(int x0, int y0, int length, LedColor color) {
            for (int i = 0; i <= length; i++) 
                DrawPixel(x0 + i, y0, color);
        }

        /// <summary>
        /// Draws a vertical line. Since horizontal lines are easy this avoids running
        /// Bresenham's algorithm.
        /// </summary>
        /// <param name="x0">Start X coords</param>
        /// <param name="y0">Start Y coords</param>
        /// <param name="length">Length of line</param>
        /// <param name="color">Color of line</param>
        private void DrawFastVLine(int x0, int y0, int length, LedColor color) {
            for (int i = 0; i <= length; i++)
                DrawPixel(x0, y0 + i, color);
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <remarks>
        /// Based on code from the SmartMatrix library written by
        /// Louis Beaudoin. Check License.txt for more info.
        /// </remarks>
        /// <param name="x">Center X coords</param>
        /// <param name="y">Center Y coords</param>
        /// <param name="radius">Circle radius</param>
        /// <param name="color">Circle color</param>
        public void DrawCircle(int x, int y, int radius, LedColor color) {
            int a = radius, b = 0;
            int radiusError = 1 - a;

            if (radius == 0) {
                DrawPixel(x, y, color);
                return;
            }

            while (a >= b) {
                DrawPixel(a + x, b + y, color);
                DrawPixel(b + x , a + y, color);
                DrawPixel(-a + x, b + y, color);
                DrawPixel(-b + x, a + y, color);
                DrawPixel(-a + x, -b + y, color);
                DrawPixel(-b + x, -a + y, color);
                DrawPixel(a + x, -b + y, color);
                DrawPixel(b + x, -a + y, color);

                b++;
                if (radiusError < 0)
                    radiusError += 2 * b + 1;
                else {
                    a--;
                    radiusError += 2 * (b - a + 1);
                }
            }
        }

        /// <summary>
        /// Draws a filled circle.
        /// </summary>
        /// <remarks>
        /// Based on code from the SmartMatrix library written by
        /// Louis Beaudoin. Check License.txt for more info.
        /// </remarks>
        /// <param name="x">Center X coords</param>
        /// <param name="y">Center Y coords</param>
        /// <param name="radius">Circle Radius</param>
        /// <param name="outlineColor">Outline Color</param>
        /// <param name="fillColor">Fill color</param>
        public void DrawCircle(int x, int y, int radius, LedColor outlineColor, LedColor fillColor) {
            int a = radius, b = 0;
            int radiusError = 1 - a;

            if (radius == 0)
                return;

            // only draw one line per row, skipping the top and bottom
            bool hlineDrawn = true;

            while (a >= b) {
                // this pair sweeps from horizontal center down
                DrawPixel(a + x, b + y, outlineColor);
                DrawPixel(-a + x, b + y, outlineColor);
                DrawLine((a - 1) + x, (-a + 1) + x, (a - 1) + x, b + y, fillColor);

                // this pair sweeps from bottom up
                DrawPixel(b + x, a + y, outlineColor);
                DrawPixel(-b + x, a + y, outlineColor);

                // this pair sweeps from horizontal center up
                DrawPixel(-a + x, -b + y, outlineColor);
                DrawPixel(a + x, -b + y, outlineColor);
                DrawLine((a - 1) + x, (-a + 1) + x, (a - 1) + x, -b + y, fillColor);

                // this pair sweeps from top down
                DrawPixel(-b + x, -a + y, outlineColor);
                DrawPixel(b + x, -a + y, outlineColor);

                if (b > 1 && !hlineDrawn) {
                    DrawLine((b - 1) + x, (-b + 1) + x, (b - 1) + x, a + y, fillColor);
                    DrawLine((b - 1) + x, (-b + 1) + x, (b - 1) + x, -a + y, fillColor);
                    hlineDrawn = true;
                }

                b++;
                if (radiusError < 0) {
                    radiusError += 2 * b + 1;
                } else {
                    a--;
                    hlineDrawn = false;
                    radiusError += 2 * (b - a + 1);
                }
            }
        }

        /// <summary>
        /// Draws a rectangle
        /// </summary>
        /// <param name="x0">Top left corner X</param>
        /// <param name="y0">Top left corner Y</param>
        /// <param name="x1">Bottom right corner X</param>
        /// <param name="y1">Bottom right corner Y</param>
        /// <param name="color">Rectangle color</param>
        public void DrawRectangle(int x0, int y0, int x1, int y1, LedColor color) {
            int width = Math.Abs(x0 - x1);
            int height = Math.Abs(y0 - y1);

            DrawFastHLine(x0, y0, width, color);
            DrawFastHLine(x0, y1, width, color);
            DrawFastVLine(x0, y0, height, color);
            DrawFastVLine(x1, y0, height, color);
        }

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="x0">Top left corner X</param>
        /// <param name="y0">Top left corner Y</param>
        /// <param name="x1">Bottom right corner X</param>
        /// <param name="y1">Bottom right corner Y</param>
        /// <param name="outlineColor">Rectangle outline color</param>
        /// <param name="fillColor">Rectangle fill color</param>
        public void DrawRectangle(int x0, int y0, int x1, int y1, LedColor outlineColor, LedColor fillColor) {
            int width = Math.Abs(x0 - x1);
            int height = Math.Abs(y0 - y1);

            if (height > width) {
                for (int i = 0; i <= width; i++)
                    DrawFastVLine(x0 + i, y0, height, fillColor);
            } else {
                for (int i = 0; i <= height; i++)
                    DrawFastHLine(x0, y0 + i, width, fillColor);
            }

            // Draw outline
            if (outlineColor != fillColor)
                DrawRectangle(x0, y0, x1, y1, outlineColor);
        }

        /// <summary>
        /// Fills the entire display with one color
        /// </summary>
        /// <param name="color">Fill color</param>
        public void FillDisplay(LedColor color) {
            for (int i = 0; i < Size; i++)
                _frontBuffer[i] = color;
        }

        /// <summary>
        /// Clears the display
        /// </summary>
        public void ClearDisplay() {
            FillDisplay(LedColor.Black);
        }
        
        /// <summary>
        /// Draws a bitmap to the display.
        /// It will be scaled to the given size
        /// </summary>
        /// <param name="bitmap">Bitmap to draw</param>
        /// <param name="x">Start X coords</param>
        /// <param name="y">Start Y coords</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public void DrawBitmap(Bitmap bitmap, int x, int y, int width, int height) {
            using (Bitmap scaled = Utils.ScaleBitmap(bitmap, width, height)) {
                for (int bx = 0; bx < width; bx++)
                    for (int by = 0; by < height; by++)
                        DrawPixel(x + bx, y + by, scaled.GetPixel(bx, by));
            }
        }

        /// <summary>
        /// Draws a bitmap to the display.
        /// </summary>
        /// <param name="bitmap">Bitmap to draw</param>
        /// <param name="x">Start X coords</param>
        /// <param name="y">Start Y coords</param>
        public void DrawBitmap(Bitmap bitmap, int x, int y) {
            for (int bx = 0; bx < bitmap.Width; bx++)
                for (int by = 0; by < bitmap.Height; by++)
                    DrawPixel(x + bx, y + by, bitmap.GetPixel(bx, by));
        }

        /// <summary>
        /// Draws a character at given coordinates.
        /// </summary>
        /// <param name="x">X (top left of character)</param>
        /// <param name="y">Y (top left of character)</param>
        /// <param name="color">Foreground color</param>
        /// <param name="character">Character to draw</param>
        /// <param name="font">Font to use (6x8), must be a byte array (byte per horizontal line). Starts at SPACE (32)</param>
        public void DrawCharacter(int x, int y, LedColor color, char character, byte[,] font) {
            for (short cy = 0; cy < 8; cy++)
                for (short cx = 0; cx < 6; cx++) 
                    if (((font[character - 32, cx] >> cy) & 0x01) != 0)
                        DrawPixel(x + cx, y + cy, color);
        }

        /// <summary>
        /// Draws a string at given coordinates
        /// </summary>
        /// <param name="x">X (top left)</param>
        /// <param name="y">Y (top left)</param>
        /// <param name="color">Foreground color</param>
        /// <param name="text">Text to print</param>
        /// <param name="gap">Gap between characters in pixels. It can be negative</param>
        /// <param name="font">Font to use (8x8), must be a byte[128, 8] array, 1 byte/line, ascii addresses</param>
        public void DrawString(int x, int y, LedColor color, String text, int gap, byte[,] font) {
            int i = 0; // Index
            foreach (char c in text) {
                DrawCharacter(x + (i * (6 + gap)), y, color, c, font);
                i++;
            }
        }
    }
}
