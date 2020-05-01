using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageCompress
{
    class Program
    {
        static void Main(string[] args)
        {
            //Commandline arguments
            /*
             * imagecompress [options] source destination [quality]
             * imagecompress [options] sourceDirectory destinationDirectory [quality]
             * source   the image file to be compressed
             * destination  specify the directory or filename of the new file
             * options:
             *      
             *          
            */

            //handle arguments
            if (args.Length == 0)
            {
                PrintHelpText(); return;
            }
            long quality;
            var qualitySpecified = long.TryParse(args[args.Length - 1], out quality);
            quality = qualitySpecified ? quality : 80;
            int offset = qualitySpecified ? 1 : 0;
            var outputP = args[args.Length - 1 - offset];
            var source = args[args.Length - 1 - offset - 1];

            var isValid = (source.IsFile() || source.IsDirectory()) && (outputP.IsDirectory() || outputP.IsFile());
            if (!isValid)
            {
                PrintHelpText(); return;
            }
            //source is a single file
            if (File.Exists(source))
            {
                if (outputP.IsFile()) CompressHandler.Compress(source, outputP, quality);
                else
                {
                    if (Directory.Exists(outputP))
                    {
                        try
                        {
                            CompressHandler.Compress(source, Path.Combine(outputP, Path.GetFileNameWithoutExtension(source)) + ".jpg", quality);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error with{source},{e.Message},{e.StackTrace}");
                        }
                    }
                    else Console.WriteLine("Directory not exists" + outputP);
                }
            }
            //source is a Directory
            else if (Directory.Exists(source))
            {
                if (!Directory.Exists(outputP)) Directory.CreateDirectory(outputP);
                var filesList = new DirectoryAnalyst(source).GetImageFilesList();
                Parallel.ForEach(filesList, (filename) =>
                  {
                      var relativePath = Path.GetRelativePath(source, filename);
                      var targetPath = Path.GetFullPath(relativePath, outputP);
                      var dir = Path.GetDirectoryName(targetPath);
                      if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                      try
                      {
                          CompressHandler.Compress(filename, targetPath, quality);
                      }
                      catch (Exception e)
                      {
                          Console.WriteLine($"Error with{filename},{e.Message},{e.StackTrace}");
                      }
                      Console.WriteLine("saved as " + targetPath);
                  });
            }
            else
            {
                Console.WriteLine("File or directory not exists" + source);
            }
        }
        static void PrintHelpText()
        {
            Console.WriteLine(@"Help:
* imagecompress source destination [quality]
* imagecompress[options] sourceDirectory destinationDirectory [quality]
* source   the image file to be compressed
* destination  specify the directory or filename of the new file
* quality   quality argument for the jpeg encoder. Default:80
*
*");
        }
    }
}
