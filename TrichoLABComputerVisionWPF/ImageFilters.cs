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
        private int[,] grayScaledOriginal;

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

            grayScaledOriginal = result;
        }

        public Bitmap GetGrayScaledImage()
        {
            int width = grayScaledOriginal.GetLength(0);
            int height = grayScaledOriginal.GetLength(1);

            Bitmap output = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int colorValue = grayScaledOriginal[i, j];
                    Color color = Color.FromArgb(colorValue, colorValue, colorValue);
                    output.SetPixel(i, j, color);
                }
            }

            return output;
        }

        public Bitmap ApplyBradleysFilter()
        {
            int width = grayScaledOriginal.GetLength(0);
            int height = grayScaledOriginal.GetLength(1);

            Bitmap output = new Bitmap(width, height);

            int[,] intImg = new int[width, height];
            int[,] outImg = new int[width, height];

            int s = width / 8;
            int t = 15;

            for (int i = 0; i < width; i++)
            {
                int sum = 0;
                for (int j = 0; j < height; j++)
                {
                    sum += grayScaledOriginal[i, j];
                    if (i == 0)
                    {
                        intImg[i, j] = sum;
                    } 
                    else
                    {
                        intImg[i, j] = intImg[i - 1, j] + sum;
                    }
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int x1 = Math.Max(i - s / 2, 1);
                    int x2 = Math.Min(i + s / 2, width - 1);
                    int y1 = Math.Max(j - s / 2, 1);
                    int y2 = Math.Min(j + s / 2, height - 1);
                    int count = (x2 - x1) * (y2 - y1);
                    int sum = intImg[x2, y2] - intImg[x2, y1 - 1] - intImg[x1 - 1, y2] + intImg[x1 - 1, y1 - 1];
                    if((grayScaledOriginal[i, j] * count) <= (sum * (100 - t) / 100))
                    {
                        output.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    }
                    else
                    {
                        output.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    }
                }
            }

            return output;
        }
    }
}
