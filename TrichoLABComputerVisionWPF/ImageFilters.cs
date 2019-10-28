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
        private Bitmap original;
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

            original = bitmap;
            grayScaledOriginal = result;
        }

        public Bitmap ApplyBradleysFilter(double slider_s, double slider_t)
        {
            int width = grayScaledOriginal.GetLength(0);
            int height = grayScaledOriginal.GetLength(1);

            int[,] intImg = new int[width, height];
            Bitmap output = new Bitmap(width, height);

            int s = (int) (width * slider_s / 100.0);
            int t = (int) slider_t;

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

        public Bitmap ApplyGaussFilter(double[,] kernel)
        {
            int width = original.Width;
            int height = original.Height;

            Bitmap output = new Bitmap(width, height);

            double weight_sum = 0.0;

            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    weight_sum += kernel[i, j];
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    output.SetPixel(i, j, applyKernel(original, kernel, weight_sum, i, j));
                }
            }

            return output;
        }

        private Color applyKernel(Bitmap bitmap, double[,] kernel, double weight_sum, int x, int y)
        {
            double sumR = 0.0;
            double sumG = 0.0;
            double sumB = 0.0;

            int offset = kernel.GetLength(0) / 2;

            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int j = 0; j < kernel.GetLength(1); j++)
                {
                    int global_x = Math.Min(Math.Max(x + i - offset, 0), bitmap.Width - 1);
                    int global_y = Math.Min(Math.Max(y + j - offset, 0), bitmap.Height - 1);

                    Color color = bitmap.GetPixel(global_x, global_y);
                    sumR += color.R * kernel[i, j];
                    sumG += color.G * kernel[i, j];
                    sumB += color.B * kernel[i, j];
                }
            }

            return Color.FromArgb(bitmap.GetPixel(x, y).A, (int) (sumR / weight_sum), (int)(sumG / weight_sum), (int)(sumB / weight_sum));
        }
    }
}
