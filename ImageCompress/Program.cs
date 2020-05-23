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
            var destination = Path.GetFullPath(args[args.Length - 1 - offset]);
            var source = Path.GetFullPath(args[args.Length - 1 - offset - 1]);

            var isValid = (source.IsFile() || source.IsDirectory()) && (destination.IsDirectory() || destination.IsFile());
            if (!isValid)
            {
                PrintHelpText(); return;
            }
            //source is a single file
            if (File.Exists(source))
            {
                if (destination.IsFile()) CompressHandler.Compress(source, destination, quality);
                else
                {
                    if (Directory.Exists(destination))
                    {

                        var outputPath = destination.IsDirectory() ?
                            Path.Combine(destination, Path.GetFileNameWithoutExtension(source)) + ".jpg"
                            : destination;//destination could be either a directory or a file path.
                        try
                        {
                            CompressHandler.Compress(source,outputPath , quality);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error with{source},{e.Message},{e.StackTrace}");
                        }
                    }
                    else Console.WriteLine("Directory not exists" + destination);
                }
            }
            //source is a Directory
            else if (Directory.Exists(source))
            {
                if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
                var filesList = new DirectoryAnalyst(source).GetImageFilesList();
                Parallel.ForEach(filesList, (filename) =>
                  {
                      //keep directory struct
                      var relativePath = Path.GetRelativePath(source, filename);
                      var targetPath = Path.GetFullPath(relativePath, destination);

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
*   imagecompress source destination [quality]
*   source        the image file or a directory which contains files to be compressed
*   destination   specify the directory or filename of the new file
*   quality       quality argument for the jpeg encoder. Default:80
*
*");
        }
    }
}
