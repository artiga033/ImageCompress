using System.Runtime.InteropServices.ComTypes;

namespace ImageCompress.Shared
{
    public interface ICompressOption
    {
        public long Quality { get; set; }
    }
    public interface IMassCompressOption : ICompressOption
    {
        public bool NoKeepStruct { get; set; }
        public bool NoRecurse { get; set; }
    }
}