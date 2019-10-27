﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;

namespace TrichoLABComputerVisionWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageFilters ImageFilters = new ImageFilters();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImageFilters.LoadImage(new Bitmap(openFileDialog.FileName));

                Uri fileUri = new Uri(openFileDialog.FileName);
                imageOriginal.Source = new BitmapImage(fileUri);

                imageBradleyFilter.Source = BitmapToImageSource(ImageFilters.ApplyBradleysFilter(SliderBradley_s.Value, SliderBradley_t.Value));

                imageGaussFilter.Source = BitmapToImageSource(ImageFilters.ApplyGaussFilter());
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void SliderBradley_s_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LabelBradley_s.Content = string.Format("s: {0}%", SliderBradley_s.Value);
        }

        private void SliderBradley_t_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LabelBradley_t.Content = string.Format("t: {0}%", SliderBradley_t.Value);
        }

        private void ButtonRefreshBradleyFilter_Click(object sender, RoutedEventArgs e)
        {
            imageBradleyFilter.Source = BitmapToImageSource(ImageFilters.ApplyBradleysFilter(SliderBradley_s.Value, SliderBradley_t.Value));
        }
    }
}
