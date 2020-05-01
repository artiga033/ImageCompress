using System.Drawing;
using System.Drawing.Imaging;

namespace ImageCompress
{
    static class CompressHandler
    {
        private static ImageCodecInfo GetCodecInfo(string format)
        {
            var _ = ImageCodecInfo.GetImageDecoders();
            ImageCodecInfo codecInfo = null;
            foreach (var i in _)
            {
                if (i.FormatDescription.Equals(format))
                {
                    codecInfo = i;
                    break;
                }
            }
            return codecInfo;
        }
        private static ImageCodecInfo GetCodecInfo(ImageFormat format)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].FormatID == format.Guid)
                    return encoders[j];
            }
            return null;
        }
        private static EncoderParameters GetEncoderParameters(long quality)
        {
            EncoderParameter p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            EncoderParameters parameters = new EncoderParameters();
            parameters.Param[0] = p;
            return parameters;
        }
        public static void Compress(string filename,string outputfile,long quality)
        {
            var img = Image.FromFile(filename);
            var output = new Bitmap(img.Size.Width,img.Size.Height);
            Graphics g = Graphics.FromImage(output);
            g.DrawImage(img, new Rectangle(0, 0, img.Size.Width, img.Size.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            g.Dispose();
            img.Dispose();
            var codec = GetCodecInfo(ImageFormat.Jpeg);
            var eps = GetEncoderParameters(quality);

            output.Save(outputfile, codec, eps);
            output.Dispose();
        }

    }
}
