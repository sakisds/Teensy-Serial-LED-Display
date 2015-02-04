using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teensy_LED_Display_Library {
    public abstract class Utils {

        /// <summary>
        /// Scales a bitmap to the given size.
        /// </summary>
        /// <param name="bitmap">Bitmap to scale</param>
        /// <param name="width">New Width</param>
        /// <param name="height">New Height</param>
        /// <returns>Scaled bitmap</returns>
        public static Bitmap ScaleBitmap(Bitmap bitmap, int width, int height) {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result)) {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(bitmap, 0, 0, width, height);
            }

            return result;
        }
    }
}
