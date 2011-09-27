using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Threading;


namespace ResizeMe
{
    
    public class ImageEngine
    {
        private string m_SourceFolder;
        private string m_DestFolder;
        private string m_OutputFile;
        private double m_ScalePercent;
        public UpdateProgress progressDelegate;
        private List<String> FilesToProcess;
        
        private ImageEngine() { }
        public ImageEngine(string sourceFolder, string destFolder,double scale)
        {
            m_SourceFolder = sourceFolder;
            m_DestFolder = destFolder;
            m_ScalePercent = scale;
            FilesToProcess = new List<string>();
        }
        public void Process() 
        {
            int count = 1;
            foreach (string File in FilesToProcess)
            {
                m_OutputFile = m_DestFolder + "\\" + Path.GetFileName(File);
                progressDelegate.Invoke(count, File);
                Bitmap img = new Bitmap(File);
                Size size = new Size(0, 0);
                Bitmap newImage = resizeImage(img, size, (float)m_ScalePercent);
                SaveImage(newImage, 100L);
                img.Dispose();
                newImage.Dispose();
                count++;
                
            }
            
        }

        private void SaveImage(Bitmap img, long quality) 
        {
            
            EncoderParameter qty = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParam = new EncoderParameters(1);
            encoderParam.Param[0] = qty;

            img.Save(m_OutputFile, jpegCodec, encoderParam);
            img.Dispose();
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

        public void GetFilesInDirectory()
        {
            string[] files = Directory.GetFiles(m_SourceFolder);
            for (int i = 0; i != files.Length; ++i)
            {
                string ext = System.IO.Path.GetExtension(files[i]);
                if (ext.ToLower() == ".jpg")
                    FilesToProcess.Add(System.IO.Path.GetFullPath(files[i]));
            }
        }
    }

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
