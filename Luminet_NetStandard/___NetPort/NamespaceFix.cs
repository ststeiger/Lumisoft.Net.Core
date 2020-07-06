
namespace System.Drawing 
{
    namespace Imaging
    {
        // http://referencesource.microsoft.com/#System.Drawing/commonui/System/Drawing/Advanced/ImageFormat.cs
        public enum ImageFormat
        {
            Jpeg,
            Png,
            Bmp,
            Gif
        }
    }


    public class Image
    {

        protected SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> m_image;

        public Image() { }

        public Image(System.IO.Stream strm)
        {
            this.m_image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(strm);
        }

        public static Image FromStream(System.IO.Stream strm)
        {
            return new System.Drawing.Image(strm);
        }


        public void Save(System.IO.Stream strm, System.Drawing.Imaging.ImageFormat format)
        {
            SixLabors.ImageSharp.Formats.IImageEncoder enc = null;

            if (format == System.Drawing.Imaging.ImageFormat.Jpeg)
                enc = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
            else if (format == System.Drawing.Imaging.ImageFormat.Png)
                enc = new SixLabors.ImageSharp.Formats.Png.PngEncoder();
            else if (format == System.Drawing.Imaging.ImageFormat.Gif)
                enc = new SixLabors.ImageSharp.Formats.Gif.GifEncoder();
            else if (format == System.Drawing.Imaging.ImageFormat.Bmp)
                enc = new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder();

            this.m_image.Save(strm, enc);
        }


    }


}
