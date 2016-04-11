using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Delta.IO
{
    /// <summary>台達內部常用的路徑相關功能。</summary>
    public static class Path
    {
        /// <summary>取得路徑內最後一組名字。(會自動將'\','/'區隔開其他的文字)</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetLastName(string path)
        {
            return path.Split(System.IO.Path.DirectorySeparatorChar
                            , System.IO.Path.AltDirectorySeparatorChar).Last();
        }

        /// <summary>檢查是否此路徑為資料夾</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectory(string path)
        {
            if (System.IO.Directory.Exists(path)) return true; // is a directory 
            return false;
        }

        /// <summary>??? 到時候問軍宇命名法是否有更精準的名稱。</summary>
        /// <param name="source"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static string Sub(string source, string reference)
        {
            string str = source;
            str = str.Replace(reference, string.Empty);
            if(str.Count() > 0)
            {
                while (str[0] == '\\' || str[0] == '/')
                {
                    str = str.Substring(1);
                    if (str.Count() <= 0) break;
                }
            }
            return str;
        }

        /// <summary> 比較兩路徑是否相同，相同則回傳true。</summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static bool Compare(string path1, string path2)
        {
            return Normalize(path1) == Normalize(path2);
        }

        private static string Normalize(string path)
        {
            return System.IO.Path.GetFullPath(path)
                       .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        /// <summary>
        /// 將路徑內所有DirectorySeparatorChar('\\')改變為AltDirectorySeparatorChar('/')
        /// </summary>
        /// <param name="path">路徑</param>
        /// <returns></returns>
        public static string ConvertToAltDirectorySeparator(string path)
        {
            return path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// 將路徑內所有AltDirectorySeparatorChar('/')改變為DirectorySeparatorChar('\\')
        /// </summary>
        /// <param name="path">路徑</param>
        /// <returns></returns>
        public static string ConvertToDirectorySeparator(string path)
        {
            return path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
        }

        #region " Methods - Get "

        /// <summary>取得一個暫存檔案的路徑。</summary>
        /// <returns>取得一個暫存檔案的路徑。</returns>
        public static string GetTempPath()
        {   
            string tempPath = System.IO.Path.GetTempPath();
            int lastIndex = tempPath.LastIndexOf('\\');
            return tempPath.Insert(lastIndex, @"\Delta");
        }


        /// <summary>取得一個暫存檔案的檔案。</summary>
        /// <returns>取得一個暫存檔案的檔案。</returns>
        public static string GetTempFile()
        {
            return System.IO.Path.GetTempFileName();
        }


        #endregion

    }
}
