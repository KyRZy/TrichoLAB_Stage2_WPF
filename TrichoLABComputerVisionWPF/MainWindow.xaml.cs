using Microsoft.Win32;
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
using System.Text.RegularExpressions;

namespace TrichoLABComputerVisionWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageFilters ImageFilters = new ImageFilters();
        double[,] kernel = new double[3, 3];

        Bitmap BradleysImage, GaussImage;

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

                BradleysImage = ImageFilters.ApplyBradleysFilter(SliderBradley_s.Value, SliderBradley_t.Value);
                imageBradleyFilter.Source =  BitmapToImageSource(BradleysImage);

                KernelValidation();
                GaussImage = ImageFilters.ApplyGaussFilter(kernel);
                imageGaussFilter.Source = BitmapToImageSource(GaussImage);

                SliderBradley_s.IsEnabled = true;
                SliderBradley_t.IsEnabled = true;
                ButtonRefreshBradleyFilter.IsEnabled = true;
                ButtonSaveBradleyToFile.IsEnabled = true;

                ButtonLoadKernel.IsEnabled = true;
                ButtonSaveKernel.IsEnabled = true;
                ButtonRefreshGaussFilter.IsEnabled = true;
                ButtonSaveGaussToFile.IsEnabled = true;

                TextBoxGauss00.IsEnabled = true;
                TextBoxGauss01.IsEnabled = true;
                TextBoxGauss02.IsEnabled = true;
                TextBoxGauss10.IsEnabled = true;
                TextBoxGauss11.IsEnabled = true;
                TextBoxGauss12.IsEnabled = true;
                TextBoxGauss20.IsEnabled = true;
                TextBoxGauss21.IsEnabled = true;
                TextBoxGauss22.IsEnabled = true;

            }
        }

        private void KernelValidation()
        {
            double k00, k01, k02, k10, k11, k12, k20, k21, k22;
            if (double.TryParse(TextBoxGauss00.Text, out k00) &&
                double.TryParse(TextBoxGauss01.Text, out k01) &&
                double.TryParse(TextBoxGauss02.Text, out k02) &&
                double.TryParse(TextBoxGauss10.Text, out k10) &&
                double.TryParse(TextBoxGauss11.Text, out k11) &&
                double.TryParse(TextBoxGauss12.Text, out k12) &&
                double.TryParse(TextBoxGauss20.Text, out k20) &&
                double.TryParse(TextBoxGauss21.Text, out k21) &&
                double.TryParse(TextBoxGauss22.Text, out k22))
            {
                
                kernel[0, 0] = k00;
                kernel[0, 1] = k01;
                kernel[0, 2] = k02;
                kernel[1, 0] = k10;
                kernel[1, 1] = k11;
                kernel[1, 2] = k12;
                kernel[2, 0] = k20;
                kernel[2, 1] = k21;
                kernel[2, 2] = k22;
            } 
            else
            {
                MessageBox.Show("Podany kernel powinien składać się z liczb rzeczywistych.\n\nZa moment zostaną przywrócone domyślne wartości kernela.");

                kernel[0, 0] = 0.0;
                kernel[0, 1] = 0.2;
                kernel[0, 2] = 0.0;
                kernel[1, 0] = 0.2;
                kernel[1, 1] = 0.2;
                kernel[1, 2] = 0.2;
                kernel[2, 0] = 0.0;
                kernel[2, 1] = 0.2;
                kernel[2, 2] = 0.0;

                TextBoxGauss00.Text = kernel[0, 0].ToString();
                TextBoxGauss01.Text = kernel[0, 1].ToString();
                TextBoxGauss02.Text = kernel[0, 2].ToString();
                TextBoxGauss10.Text = kernel[1, 0].ToString();
                TextBoxGauss11.Text = kernel[1, 1].ToString();
                TextBoxGauss12.Text = kernel[1, 2].ToString();
                TextBoxGauss20.Text = kernel[2, 0].ToString();
                TextBoxGauss21.Text = kernel[2, 1].ToString();
                TextBoxGauss22.Text = kernel[2, 2].ToString();
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
            BradleysImage = ImageFilters.ApplyBradleysFilter(SliderBradley_s.Value, SliderBradley_t.Value);
            imageBradleyFilter.Source = BitmapToImageSource(BradleysImage);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ButtonRefreshGaussFilter_Click(object sender, RoutedEventArgs e)
        {
            KernelValidation();
            GaussImage = ImageFilters.ApplyGaussFilter(kernel);
            imageGaussFilter.Source = BitmapToImageSource(GaussImage);
        }

        private void ButtonSaveKernel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".text";
            dlg.Filter = "Text documents (.txt)|*.txt";

            if (dlg.ShowDialog() == true)
            {
                KernelValidation();
                string output = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", kernel[0, 0], kernel[0, 1], kernel[0, 1], kernel[1, 0], kernel[1, 1], kernel[1, 2], kernel[2, 0], kernel[2, 1], kernel[2, 2]);
                File.WriteAllText(dlg.FileName, output);
            }
        }

        private void ButtonLoadKernel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string text = File.ReadAllText(openFileDialog.FileName);
                string[] KernelValues = text.Split();

                if(KernelValues.Length != 9)
                {
                    return;
                }

                TextBoxGauss00.Text = KernelValues[0];
                TextBoxGauss01.Text = KernelValues[1];
                TextBoxGauss02.Text = KernelValues[2];
                TextBoxGauss10.Text = KernelValues[3];
                TextBoxGauss11.Text = KernelValues[4];
                TextBoxGauss12.Text = KernelValues[5];
                TextBoxGauss20.Text = KernelValues[6];
                TextBoxGauss21.Text = KernelValues[7];
                TextBoxGauss22.Text = KernelValues[8];

            }                
        }

        private void ButtonSaveBradleyImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG (.png)|*.png";

            if (dlg.ShowDialog() == true)
            { 
                BradleysImage.Save(dlg.FileName);
            }
        }

        private void ButtonSaveGaussImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG (.png)|*.png";

            if (dlg.ShowDialog() == true)
            {
                GaussImage.Save(dlg.FileName);
            }
        }


    }
}
