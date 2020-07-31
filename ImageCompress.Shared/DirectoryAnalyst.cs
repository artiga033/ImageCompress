using System.Collections.Generic;
using System.IO;

namespace ImageCompress.Shared
{
    public static class DirectoryAnalyst
    {
        /// <summary>
        /// 获取此目录(其子目录)所有图片文件的列表
        /// </summary>
        /// <param name="includeChild">是否包含子目录文件</param>
        /// <returns></returns>
        public static List<string> GetImageFilesList(string directory, bool includeChild = true)
        {
            List<string> filenameList = new List<string>();

            foreach (var i in Directory.GetFiles(directory, "*", includeChild ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                filenameList.TryAddImgFile(i);
            }
            return filenameList;
        }
    }
}
