using System.Collections.Generic;
using System.IO;

namespace ImageCompress
{
    class DirectoryAnalyst
    {
        public DirectoryAnalyst(string dir)
        {
            if (Directory.Exists(dir)) this.dir = dir;
            else throw new DirectoryNotFoundException();
        }
        private string dir;
        /// <summary>
        /// 获取此目录(其子目录)所有图片文件的列表
        /// </summary>
        /// <param name="includeChild">是否包含子目录文件</param>
        /// <returns></returns>
        public List<string> GetImageFilesList(bool includeChild = true)
        {
            List<string> filenameList = new List<string>();

            void F(string dir)
            {
                var files = Directory.GetFiles(dir);
                foreach (var item in files)
                {
                    filenameList.TryAddImgFile(item);
                }
                var childDirs = Directory.GetDirectories(dir);
                foreach (var item in childDirs)
                {
                    F(item);
                }
            };
            F(this.dir);
            return filenameList;
        }
    }
}
