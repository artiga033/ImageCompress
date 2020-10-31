using System;
using System.Collections.Generic;
using System.Text;

namespace ImageCompress.Shared
{
    public class CompressOption : ICompressOption
    {
        public static CompressOption FromInterface(ICompressOption iOption) => (CompressOption)iOption;
        public long Quality { get; set; } = 80;
    }
    public class MassCompressOption : CompressOption, IMassCompressOption
    {
        public bool NoKeepStruct { get; set; }
        public bool NoRecurse { get; set; }
    }
}
