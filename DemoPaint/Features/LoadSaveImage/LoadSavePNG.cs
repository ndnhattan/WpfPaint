using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace DemoPaint.Features.LoadSaveImage
{
    public class LoadSavePNG
    {
        public static void SaveCanvasToPng(Canvas canvas)
        {
            // Render the canvas to a bitmap
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Default);
            rtb.Render(canvas);

            // Convert the bitmap to a PNG file
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            // Save the PNG file using a SaveFileDialog
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PNG Image|*.png";
            if (saveDialog.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create))
                {
                    pngEncoder.Save(fs);
                }
            }
        }

        public static void LoadPngToCanvas(Canvas canvas, ref Image img)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "PNG Image|*.png";
            if (openDialog.ShowDialog() == true)
            {
                string filename = openDialog.FileName;

                byte[] imageBytes;
                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    imageBytes = new byte[stream.Length];
                    stream.Read(imageBytes, 0, (int)stream.Length);
                }

                BitmapImage bi = new BitmapImage();
                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    bi.BeginInit();
                    bi.StreamSource = memoryStream;
                    bi.CacheOption = BitmapCacheOption.OnLoad; // Ensures the image is fully loaded into memory
                    bi.EndInit();
                }

                img = new Image();
                img.Source = bi;

                // Set the position of the image within the Canvas
                Canvas.SetLeft(img, 0); // Adjust as needed
                Canvas.SetTop(img, 0); // Adjust as needed

                canvas.Children.Add(img);
            }
        }
    }
}
