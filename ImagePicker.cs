using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;

namespace SnakeWpf
{
    public class ImagePicker
    {
        int imageNo = 0;
        //public ImageBrush ib = new ImageBrush();
        public List<ImageBrush> images = new List<ImageBrush>();
        public PixabayResponse pixabayResponse { get; set; }

        public int currentSnakePhotoId;

        public void ImageDataPickerMethod()
        {

            pixabayResponse = new PixabayResponse();
            try
            {
                string json = new WebClient()
                    .DownloadString($"https://pixabay.com/api/?key=15875743-f2d5b9b261bbd2a7639c0ce0c&per_page=5&q=snake&image_type=photo");
                pixabayResponse = JsonConvert.DeserializeObject<PixabayResponse>(json);
            }
            catch (Exception)
            {
                // TODO: Info about UrlError
                pixabayResponse = null;
            }
        }

        //Used After FoodContact and after randomize
        public ImageBrush SaveImage()
        {
            //ImageBrush ib = new ImageBrush();
            ImageBrush ib = new ImageBrush();
            int id;
            id = DateTime.Now.Second % 5;

            currentSnakePhotoId = id;
            string url = pixabayResponse.hits[id].previewURL;

            //Because URL Pixabay doestn't work I'm converting from previewURL to 640 picture.
            url = url.Remove(url.Length - 7) + pixabayResponse.hits[imageNo].webformatWidth + ".jpg";

            //Convert to bitmap: But for this app not necesarry.
            ib.ImageSource = new BitmapImage(new Uri(url));

            //Background can be set from BitmapImage. BUT Exercice says BITMAP (not BitmapImage), so below conversion
            BitmapImage bi = new BitmapImage(new Uri(url));

            //Below converting BitmapImage -> Bitmap -> BitmapImage (becouse of instruction in exercise)
            //For setting Canvas Background it's not required.
            //Seting BITMAP to canvas bacground by uncomment below code.

            //ib.ImageSource = ConvertToBM(bm(bi));

            BitmapImage ConvertToBM(Bitmap src)
            {
                MemoryStream ms = new MemoryStream();
                ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }

            Bitmap bm(BitmapImage bitmapImage)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                    enc.Save(outStream);
                    Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                    return new Bitmap(bitmap);
                };
            }

            return ib;
        }
    }
}
