using System.Collections.Generic;
using System.IO;

namespace ImageCompress
{
    static class Extensions
    {
        /// <summary>
        /// 此方法判断字符串是否代表文件夹 而不是判断文件夹是否存在
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDirectory(this string str)
        {
            if (str[^1] == Path.DirectorySeparatorChar) return true; //ends with / or \
            return Directory.Exists(str);//or is an existing directory
        }
        /// <summary>
        /// 此方法判断字符串是否代表文件 而不是判断文件是否存在
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsFile(this string str) => !str.IsDirectory();

        /// <summary>
        /// 向列表添加图片文件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filename"></param>
        public static void TryAddImgFile(this List<string> list, string filename)
        {
            string extension = Path.GetExtension(filename).ToLower();
            if (extension == ".jpg" || extension == ".jpeg" || extension == ".bmp" || extension == ".png")
            {
                list.Add(filename);
            }
        }
    }
}
