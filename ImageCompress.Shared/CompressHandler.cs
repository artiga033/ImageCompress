using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ImageCompress.Shared
{
    public static class CompressHandler
    {
        private static ImageCodecInfo GetCodecInfo(string format)
        {
            var dcs = ImageCodecInfo.GetImageDecoders();
            ImageCodecInfo codecInfo = null;
            foreach (var i in dcs)
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
        private static EncoderParameters GetQualityEncoderParameters(long quality)
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
        /// <exception cref="NotSupportedException">Image.FromFile 引发的 OutOfMemoryException</exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="System.ArgumentException"></exception>

        public static void CompressSingle(string filename, string outputPath, ICompressOption option)
        {
            Image img;
            try
            {
                img = Image.FromFile(filename);
            }
            /*  OutOfMemoryException
                The file does not have a valid image format.
                -or-
                GDI+ does not support the pixel format of the file.
            --MSDN : https://docs.microsoft.com/en-us/dotnet/api/system.drawing.image.fromfile?view=dotnet-plat-ext-3.1
            */
            catch (OutOfMemoryException e)
            {
                throw new NotSupportedException("Not Supported file format or out of memory.(Mostly the former)", e);
            }
            var output = new Bitmap(img.Size.Width, img.Size.Height);
            Graphics g = Graphics.FromImage(output);
            g.DrawImage(img, new Rectangle(0, 0, img.Size.Width, img.Size.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            g.Dispose();
            img.Dispose();
            var codec = GetCodecInfo(ImageFormat.Jpeg);
            var eps = GetQualityEncoderParameters(option.Quality);


            var realPath = outputPath.IsDirectory() ?
                Path.Combine(outputPath, Path.GetFileNameWithoutExtension(filename)) + ".jpg"
                : outputPath;//outputPath could be either a directory or a file path.



            output.Save(realPath, codec, eps);
            output.Dispose();
        }

        /// <summary>
        /// 通过指定的 MassCompressOption 实例执行批量文件压缩
        /// </summary>
        /// <exception cref="AggregateException"></exception>
        public static void CompressMultiple(IEnumerable<string> files, string outputPath, IMassCompressOption option)
        {
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);


            // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
            var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();
            // Execute the complete loop and capture all exceptions.
            Parallel.ForEach(files, file =>
            {
                try
                {
                    CompressSingle(file, outputPath, option);
                }
                // Store the exception and continue with the loop.
                catch (Exception e)
                {
                    e.Source = file;
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
        public static void CompressMultiple(string sourceDirectory, string outputPath, IMassCompressOption option)
        {
            var list = DirectoryAnalyst.GetImageFilesList(sourceDirectory, !option.NoRecurse);
            if (option.NoKeepStruct)
            {
                CompressMultiple(list, outputPath, option);
            }
            //分组目录结构
            var query = from file in list
                            //分组的Key ::: -文件所在目录 与 源目录 的相对路径  相对于  输出目录  即 最终输出文件应在的目录
                        group file by Path.GetFullPath(Path.GetRelativePath(sourceDirectory, Path.GetDirectoryName(file)), outputPath) into fileGroup
                        //orderby fileGroup.Key
                        select fileGroup;
            var queue = new Queue<Exception>();
            foreach (var i in query)
            {
                try
                {
                    CompressMultiple(i, i.Key, option);
                }
                catch (AggregateException e)
                {
                    foreach (var item in e.InnerExceptions)
                    {
                        queue.Enqueue(item);
                    }
                }
            }
            if (queue.Count > 0) throw new AggregateException(queue);
        }
    }
}
