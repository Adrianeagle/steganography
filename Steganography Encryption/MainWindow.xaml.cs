using Microsoft.Win32;
using System.Drawing;
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
using System.IO;

namespace Steganography_Encryption
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //First Tab
        bool textBox_TextInput = false;
        bool textBox_ImageInput = false;

        bool ImageLoaded_Encryption = false;
        string ImageFormat = "";
        System.Drawing.Bitmap ImageToEncrypt = null;

        //Second Tab
        System.Drawing.Bitmap ImageToDecode = null;
        bool ImageLoaded_Decryption = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TabEncryptClick_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Height = 600;
            Application.Current.MainWindow.Width = 902.5;
        }

        private void TabDecryptClick_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Height = 600;
            Application.Current.MainWindow.Width = 1300;
            tabControl.Width = 1263;
        }

        private void TabPassGenClick_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Height = 600;
            Application.Current.MainWindow.Width = 800;
        }

        private void buttonLoadImage_MouseClick(object sender, RoutedEventArgs e)
        {
            //Load Image to Encrypt --> 1st Tab
            OpenFileDialog selectImageForEncryption = new OpenFileDialog();

            selectImageForEncryption.Multiselect = false;
            selectImageForEncryption.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            selectImageForEncryption.ShowDialog();

            if (selectImageForEncryption.FileName.Length > 0)
            {
                ImageToEncrypt = new Bitmap(System.Drawing.Image.FromFile(selectImageForEncryption.FileName));
                ImageBox_imageEncrypt.Source = ToBitmapImage(ImageToEncrypt);
                ImageFormat = selectImageForEncryption.FileName.Substring(selectImageForEncryption.FileName.Length - 3); //Last 3 characters == jpg, png ...
                ImageLoaded_Encryption = true;
            }
        }

        private void buttonSaveImage_MouseClick(object sender, RoutedEventArgs e)
        {
            //Save Encrypted Image --> 1st Tab
            if (ImageLoaded_Encryption != false)
            {
                SaveFileDialog saveImageDialog = new SaveFileDialog();

                saveImageDialog.Filter = "Bitmap Image|*.bmp";// Jpeg Image|*.jpg|PNG Image|*.png";
                saveImageDialog.Title = "Save Encrypted Image";
                saveImageDialog.FileName = "encrypted_image";
                saveImageDialog.ShowDialog();

                switch (ImageFormat)
                {
                    case "bmp":
                        ImageToEncrypt.Save(saveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    //case "png":
                    //    ImageToEncrypt.Save(saveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    //    break;
                    //case "jpg":
                    //    ImageToEncrypt.Save(saveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //    break;
                    default:
                        MessageBox.Show("Picutre could not be saved!", "Error", MessageBoxButton.OK);
                        break;
                }
            }
            else
            {
                MessageBox.Show("There is no image loaded!", "Error", MessageBoxButton.OK);
            }
        }

        private void textBox_TextToEncrypt_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            textBox_TextInput = true;
            textBox_TextToEncrypt.Clear();
            textBox_TextToEncrypt.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void textBox_ImageToEncrypt_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            textBox_ImageInput = true;
            textBox_ImageToEncrypt.Clear();
            textBox_ImageToEncrypt.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void buttonEncryptData_Click(object sender, RoutedEventArgs e)
        {
            //EncryptData Button --> 1st Tab

            //De verificat partea cu codul!

            if (textBox_TextInput || textBox_ImageInput)
            {
                Classes.EncryptImage_LastTwoBits ApplyEncryption = new Classes.EncryptImage_LastTwoBits(ref ImageToEncrypt, !isGrayScale(ImageToEncrypt));
                if (textBox_TextInput && textBox_TextToEncrypt.Text.Length > 0)
                {
                    if (!ApplyEncryption.EncryptText(textBox_TextToEncrypt.Text, "cod"))
                    {
                        MessageBox.Show("Encryption Failed!", "Error", MessageBoxButton.OK);
                        return;
                    }
                    ImageBox_imageEncrypt.Source = ToBitmapImage(ImageToEncrypt);
                    MessageBox.Show("Encryption Completed!");
                }
            }
            else
            {
                MessageBox.Show("There is nothing to Encrypt", "Error!", MessageBoxButton.OK);
            }
        }

        private static Boolean isGrayScale(Bitmap img)
        {
            for (Int32 h = 0; h < img.Height; h++)
                for (Int32 w = 0; w < img.Width; w++)
                {
                    System.Drawing.Color color = img.GetPixel(w, h);
                    if ((color.R != color.G || color.G != color.B || color.R != color.B) && color.A != 0)
                    {
                        return false;
                    }
                }
            return true;
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void button_LoadEncryptedImage_Click(object sender, RoutedEventArgs e)
        {
            //Load Encrypted Image --> 2nd Tab
            OpenFileDialog selectEncryptedImage = new OpenFileDialog();

            selectEncryptedImage.Multiselect = false;
            selectEncryptedImage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            selectEncryptedImage.ShowDialog();

            if (selectEncryptedImage.FileName.Length > 0)
            {
                ImageToDecode = new Bitmap(System.Drawing.Image.FromFile(selectEncryptedImage.FileName));
                ImageBox_EncodedImage.Source = ToBitmapImage(ImageToDecode);
                ImageLoaded_Decryption = true;
            }
        }

        private void buttonDecryptData_Click(object sender, RoutedEventArgs e)
        {
            //DecryptData Button --> 2nd Tab

            //Nu uita sa aplici codul!

            if (ImageLoaded_Decryption)
            {
                Classes.EncryptImage_LastTwoBits ApplyDecryption = new Classes.EncryptImage_LastTwoBits(ref ImageToDecode, !isGrayScale(ImageToDecode));
                string ResultedText = "";
                ApplyDecryption.DecryptText(ref ResultedText, "cod");
                textBox_DecryptedText.Text = ResultedText;
                MessageBox.Show("Decryption Completed!");
            }
            else
            {
                MessageBox.Show("There is nothing to Decrypt", "Error!", MessageBoxButton.OK);
            }
        }
    }
}
