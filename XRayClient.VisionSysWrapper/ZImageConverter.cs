using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XRayClient.VisionSysWrapper
{
    public class ZImageConverter
    {
        private static Bitmap color = new Bitmap("color.bmp");
        private static Bitmap grey = new Bitmap("gray.bmp");

        public static Bitmap ZYImageStruct2Bitmap(ZYImageStruct img)
        {
            Bitmap c;

            bool colorflag = img.channel == 1 ? false : true;

            if (false == colorflag)
            {
                //greyimage
                c = new Bitmap(img.width, img.height, img.width * img.channel, grey.PixelFormat, img.data);
                c.Palette = grey.Palette;
            }
            else
            {
                c = new Bitmap(img.width, img.height, img.width * img.channel, color.PixelFormat, img.data);
                // c.Palette.Flags = b_color.Palette.Flags;
                //  System.Drawing.Imaging.ColorPalette palette = b_color.Palette;

                //c.Palette = b_color.Palette;
            }

            return c;
        }

        public static ZYImageStruct Bitmap2ZYImageStruct(Bitmap bmp)
        {
            return new ZYImageStruct(bmp);
        }

    }
}
