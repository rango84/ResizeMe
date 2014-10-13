using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Threading;


namespace ResizeMe.Core
{
    
    public class ImageEngine
    {
        private double _scalePercent;
        public ImageEngine(double scalePercent) 
        {
            _scalePercent = scalePercent;   
        }

        public void ProcessImage(string file, string outputFile) 
        {
            using(Bitmap img = new Bitmap(file))
            {
                Size size = new Size(0, 0);
                using(Bitmap newImage = resizeImage(img, size, (float)_scalePercent))
                {
                    SaveImage(newImage, 100L,outputFile);
                }
            }
        }

        private void SaveImage(Bitmap img, long quality, string outputFile) 
        {
            
            EncoderParameter qty = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");
            if (jpegCodec == null)
                return;
            EncoderParameters encoderParam = new EncoderParameters(1);
            encoderParam.Param[0] = qty;
            img.Save(outputFile, jpegCodec, encoderParam);
        }

        private ImageCodecInfo getEncoderInfo(string mimeType) 
        {
            ImageCodecInfo [] codecs = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < codecs.Length; ++i)
            {
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            }
            return null;
        }

        private Bitmap resizeImage(Image originalImage, Size size, float percent) 
        {
            
            int sourceWidth = originalImage.Width;
            int sourceHeigh = originalImage.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            // need to implement GUI to use size as well 
            if (!(size.Height <= 0) || ! (size.Width <= 0))
            {
                nPercentW = ((float)size.Width / (float)sourceWidth);
                nPercentH = ((float)size.Height / (float)sourceHeigh);
            }

            if (percent >= 0.0)
                nPercent = (float)(percent / 100.0);
            else
            {
                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;
            }
            

            int destW = (int)(sourceWidth * nPercent);
            int destH = (int)(sourceHeigh * nPercent);

            Bitmap newImg = new Bitmap(destW, destH);
            Graphics g = Graphics.FromImage((Image)newImg);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(originalImage, 0, 0, destW, destH);
            g.Dispose();

            return newImg;
        }

    }

    //TODO: refactor this
    public static class Helper 
    {
        public static int GetValidFilesInDirectory(string folder)
        {
            string[] files = Directory.GetFiles(folder);
            int count =0;
            for (int i = 0; i != files.Length; ++i)
            {
                string ext = System.IO.Path.GetExtension(files[i]);
                if (ext.ToLower() == ".jpg")
                    count++;   
            }
            return count;
        }
    }
}
