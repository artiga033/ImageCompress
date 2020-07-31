using System;
using System.Collections.Generic;
using System.Text;

namespace ImageCompress.Shared
{
    public class CompressOption
    {
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutputPath { get; set; }
        public long Quality { get; set; }
    }
    public class MassCompressOption : CompressOption
    {
        public bool KeepDirectoryStruct { get; set; }
    }
}
