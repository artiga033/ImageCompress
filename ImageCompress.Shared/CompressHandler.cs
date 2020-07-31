using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImageCompress.Shared
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

        /// <summary>
        /// 通过指定的 CompressOption 实例执行单个文件压缩
        /// </summary>
        /// <param name="option"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static void CompressSingle(string filename, CompressOption option)
        {
            var img = Image.FromFile(filename);
            var output = new Bitmap(img.Size.Width, img.Size.Height);
            Graphics g = Graphics.FromImage(output);
            g.DrawImage(img, new Rectangle(0, 0, img.Size.Width, img.Size.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            g.Dispose();
            img.Dispose();
            var codec = GetCodecInfo(ImageFormat.Jpeg);
            var eps = GetEncoderParameters(option.Quality);


            var realPath = option.OutputPath.IsDirectory() ?
                Path.Combine(option.OutputPath, Path.GetFileNameWithoutExtension(filename)) + ".jpg"
                : option.OutputPath;//option.OutputPath could be either a directory or a file path.



            output.Save(option.OutputPath, codec, eps);
            output.Dispose();
        }

        /// <summary>
        /// 通过指定的 MassCompressOption 实例执行批量文件压缩
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static void CompressMultiple(IEnumerable<string> files, MassCompressOption option)
        {
            if (!Directory.Exists(option.OutputPath)) Directory.CreateDirectory(option.OutputPath);


            // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
            var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();
            // Execute the complete loop and capture all exceptions.
            Parallel.ForEach(files, file =>
            {
                try
                {
                    CompressSingle(file, option);
                }
                // Store the exception and continue with the loop.
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            // Throw the exceptions here after the loop completes.
            if (exceptions.Count > 0) throw new AggregateException(exceptions);

        }
        /// <summary>
        /// 通过指定的 MassCompressOption 实例执行批量文件压缩
        /// </summary>
        /// <param name="sourceDirectory">源文件夹</param>
        /// <param name="option"></param>
        public static void CompressMultiple(string sourceDirectory, MassCompressOption option)
        {
            CompressMultiple(DirectoryAnalyst.GetImageFilesList(sourceDirectory, option.KeepDirectoryStruct), option);
        }
    }
}
