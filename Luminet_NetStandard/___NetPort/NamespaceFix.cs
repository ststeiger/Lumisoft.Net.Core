
namespace System.Drawing 
{


    public class Image
    {
        private System.IO.Stream m_stream;

        public static Image FromStream(System.IO.Stream strm)
        {
            return null;
        }


        public void Save(System.IO.Stream strm, System.Drawing.Imaging.ImageFormat format)
        {
        }


    }


}


namespace System.Drawing.Imaging
{
    public enum ImageFormat
    {
        Jpeg
    }
}
