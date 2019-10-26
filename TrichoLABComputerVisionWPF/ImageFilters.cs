using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrichoLABComputerVisionWPF
{
    class ImageFilters
    {
        public int[,] GrayScaledOriginal { get; set; }

        public void ApplyBradleysFilter()
        {
            
        }

        public void LoadImage(Bitmap bitmap)
        {
            int[,] result = new int[bitmap.Width, bitmap.Height];

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    result[i, j] = pixel.R;
                }
            }

            GrayScaledOriginal = result;
        }

        public Bitmap GetImage()
        {
            int width = GrayScaledOriginal.GetLength(0);
            int height = GrayScaledOriginal.GetLength(1);

            Bitmap output = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int colorValue = GrayScaledOriginal[i, j];
                    Color color = Color.FromArgb(colorValue, colorValue, colorValue);
                    output.SetPixel(i, j, color);
                }
            }

            return output;
        }
    }
}
