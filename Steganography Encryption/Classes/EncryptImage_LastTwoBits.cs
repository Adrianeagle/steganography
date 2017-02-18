using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography_Encryption.Classes
{
    class EncryptImage_LastTwoBits
    {
        private Bitmap Image;
        private bool ColoredImage;
        private int ElementLine = 0;
        private int ElementColumn = 0;
        private int DataSpace = 0;
        private string Signature = "E69a44F";
        private string Marker = "<_M>";

        public EncryptImage_LastTwoBits(ref Bitmap ImageToEncrypt, bool ColoredImage)
        {
            this.Image = ImageToEncrypt;
            this.ColoredImage = ColoredImage;
            this.DataSpace = ImageToEncrypt.Height * ImageToEncrypt.Width;
            if (ColoredImage)
            {
                this.DataSpace *= 3;
            }
        }

        public bool EncryptText(string TextToEncrypt, string Code)
        {
            int AddSignature = 0;
            string ConvertedMessage = "";
            if (ElementLine == 0 && ElementColumn == 0)
            {
                AddSignature = Signature.Length * 4; // 4 = Pixels to encrypt one character
                ConvertedMessage += Signature;
            }
            if ((TextToEncrypt.Length + Marker.Length * 2) * 4 + AddSignature > DataSpace)
            {
                return false;
            }

            DataSpace -= (TextToEncrypt.Length + Marker.Length * 2) * 4 + AddSignature;
            //Encode Here -->
            ConvertedMessage = ConvertStringToBits(ConvertedMessage + "<_T>" + TextToEncrypt + "</T>");

            for (int y = 0; y < Image.Height; y++)
            {
                for (int x = 0; x < Image.Width; x++)
                {
                    System.Drawing.Color PixelColor = Image.GetPixel(x, y);

                    int TempR = PixelColor.R;
                    int TempG = PixelColor.G;
                    int TempB = PixelColor.B;

                    if (ColoredImage)
                    {
                        for (int channel = 0; channel < 3; channel++)
                        {
                            switch (channel)
                            {
                                case 0:
                                    if (ConvertedMessage.Length > 1)
                                    {
                                        TempR = (TempR >> 2) << 2;
                                        TempR += Convert.ToInt32((ConvertedMessage[0].ToString() + ConvertedMessage[1].ToString()), 2);
                                        ConvertedMessage = ConvertedMessage.Remove(0, 2);
                                    }
                                    break;
                                case 1:
                                    if (ConvertedMessage.Length > 1)
                                    {
                                        TempG = (TempG >> 2) << 2;
                                        TempG += Convert.ToInt32((ConvertedMessage[0].ToString() + ConvertedMessage[1].ToString()), 2);
                                        ConvertedMessage = ConvertedMessage.Remove(0, 2);
                                    }
                                    break;
                                case 2:
                                    if (ConvertedMessage.Length > 1)
                                    {
                                        TempB = (TempB >> 2) << 2;
                                        TempB += Convert.ToInt32((ConvertedMessage[0].ToString() + ConvertedMessage[1].ToString()), 2);
                                        ConvertedMessage = ConvertedMessage.Remove(0, 2);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        int TempVal = PixelColor.R;
                        if (ConvertedMessage.Length > 0)
                        {
                            TempVal = (TempVal >> 2) << 2;
                            TempVal += Convert.ToInt32((ConvertedMessage[0].ToString() + ConvertedMessage[1].ToString()), 2);
                            ConvertedMessage = ConvertedMessage.Remove(0, 2);
                        }

                        TempR = TempVal;
                        TempG = TempVal;
                        TempB = TempVal;
                    }

                    Image.SetPixel(x, y, Color.FromArgb(TempR, TempG, TempB));

                    if (ConvertedMessage.Length == 0)
                    {
                        ElementLine = y;
                        ElementColumn = x;
                        return true;
                    }
                }
            }
            return true;
        }

        public bool EncryptGrayImage(Bitmap ImageToEncrypt, string Code)
        {
            int AddSignature = 0;
            if (ElementLine == 0 && ElementColumn == 0)
            {
                AddSignature = Signature.Length * 4; // 4 = Pixels to encrypt one character
            }

            if (ColoredImage)
            {

            }
            else
            {

            }
            return true;
        }

        public bool EncryptRGBImage(Bitmap ImageToEncrypt, string Code)
        {
            int AddSignature = 0;
            if (ElementLine == 0 && ElementColumn == 0)
            {
                AddSignature = Signature.Length * 4; // 4 = Pixels to encrypt one character
            }

            if (ColoredImage)
            {

            }
            else
            {

            }
            return true;
        }

        public bool DecryptText(ref string DecryptedText, string Code)
        {
            string BitArray = "";
            for (int y = 0; y < Image.Height; y++)
            {
                for (int x = 0; x < Image.Width; x++)
                {
                    System.Drawing.Color PixelColor = Image.GetPixel(x, y);
                    if (ColoredImage)
                    {
                        BitArray += (Convert.ToString(PixelColor.R, 2).PadLeft(8, '0')).Remove(0, 6);
                        BitArray += (Convert.ToString(PixelColor.G, 2).PadLeft(8, '0')).Remove(0, 6);
                        BitArray += (Convert.ToString(PixelColor.B, 2).PadLeft(8, '0')).Remove(0, 6);
                    }
                    else
                    {
                        BitArray += (Convert.ToString(PixelColor.R, 2).PadLeft(8, '0')).Remove(0, 6);
                    }
                    if (BitArray.Length >= 8)
                    {
                        DecryptedText += ConvertBitsToChar(BitArray.Substring(0, 8));
                        BitArray = BitArray.Remove(0, 8);
                    }
                }
            }
            return true;
        }

        private string ConvertStringToBits(string TextToConvert)
        {
            string result = "";
            foreach (char letter in TextToConvert)
            {
                result += Convert.ToString((int)letter, 2).PadLeft(9, '0');
            }
            return result;
        }

        private char ConvertBitsToChar(string BitArray)
        {
            int Power = 1;
            int ResultedNumber = 0;

            char[] charArray = BitArray.ToCharArray();
            Array.Reverse(charArray);
            BitArray = new string(charArray);

            foreach (char Letter in BitArray)
            {
                if (Letter == '1')
                {
                    ResultedNumber += Power;
                }
                Power *= 2;
            }
            return (char)ResultedNumber;
        }
    }
}
