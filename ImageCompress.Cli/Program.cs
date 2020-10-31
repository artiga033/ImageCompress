using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using ImageCompress.Shared;
using McMaster.Extensions.CommandLineUtils;
namespace ImageCompress
{
    class Program : IMassCompressOption, ICompressOption
    {
        private string source;
        private string destination = ".";

        static void Main(string[] args)
        {
            CommandLineApplication.Execute<Program>(args);
        }
        void OnExecute()
        {
            //Source 不存在
            if (!(File.Exists(Source) || Directory.Exists(Source)))
            {
                Console.WriteLine($"File or directory {Source} not exist.");
                return;
            }
            //Source 为文件
            if (File.Exists(Source))
            {
                try
                {
                    CompressHandler.CompressSingle(Source, Destination, this);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error.\n " + e.Message + e.StackTrace);
                }
            }
            //Source 为目录
            else
            {
                try
                {
                    CompressHandler.CompressMultiple(Source, Destination, this);
                }
                catch (AggregateException aggregateException)
                {
                    Console.WriteLine("Error\n");
                    Console.WriteLine("Failed attempts (totally {0}):",aggregateException.InnerExceptions.Count);

                    foreach (var e in aggregateException.InnerExceptions)
                    {
                        Console.WriteLine($"<{e.Source}>failed because:{e.Message}|");
                    }
                }
                Console.WriteLine("Finished!");
            }
        }
        [Required]
        [Argument(0, "source", "Path to the file/folder containing raw files ")]
        public string Source { get => source; set => source = Path.GetFullPath(value); }

        [Required]
        [Argument(1, "destination", "Path where the output will be \tDefault: .")]
        public string Destination { get => destination; set => destination = Path.GetFullPath(value); }
        [Option(Description = "The \"quality\" parameter for the jpeg encoder \t Default: 80")]
        public long Quality { get; set; } = 80;

        [Option(Description = "If specified, the output folder won't keep the raw folder's directory struct. ", ShortName = "")]
        public bool NoKeepStruct { get; set; }

        [Option(Description = "If specifiled, images in raw folder's child folders won't be included. ", ShortName = "")]
        public bool NoRecurse { get; set; }
    }
}
