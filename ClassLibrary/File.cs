using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
//using System.Windows.Forms;

namespace Delta.IO
{

    /// <summary>讀寫檔案相關工具。</summary>
    public static class File
    {

        #region " Definition "

        /// <summary>檔案編碼格式。</summary>
        public enum Encode
        {
            /// <summary>編碼格式 UTF7 。</summary>
            UTF7,
            /// <summary>編碼格式 UTF8 。</summary>
            UTF8,
            /// <summary>編碼格式 UTF32 。</summary>
            UTF32,
            /// <summary>編碼格式 Unicode 。</summary>
            Unicode,
            /// <summary>預設。</summary>
            Default,
            /// <summary>編碼格式 ASCII 。</summary>
            ASCII,
            /// <summary>編碼格式 BigEndianUnicode 。</summary>
            BigEndianUnicode,
        }

        /// <summary>刪除檔案日期類型。</summary>
        public enum DeleteDateType
        {
            /// <summary>當前時間日期之前。</summary>
            Before,
            /// <summary>當前時間日期之後。</summary>
            After
        }

        /// <summary>資料夾時間型態。</summary>
        public enum FolderTimeType
        {
            /// <summary>資料夾建立日期。</summary>
            CreateTime,
            /// <summary>資料夾最後存取日期。</summary>
            LastAccessTime,
            /// <summary>資料夾最後寫入日期。</summary>
            LastWriteTime,
        }

        /// <summary>檔案大小單位。</summary>
        public enum FileSizeUnitType
        {
            /// <summary>Byte。</summary>
            Byte,
            /// <summary>KB。</summary>
            KB,
            /// <summary>MB。</summary>
            MB,
            /// <summary>GB。</summary>
            GB,
        }

        #endregion

        #region " Properties "

        /// <summary>取得，硬碟鎖定物件。</summary>
        public static object DiskLock
        {
            get { return File.g_objDiskLock; }
        }
        private static object g_objDiskLock = new object();

        #endregion

        #region " Methods - Write/Read/Move/Create/Delete with File/Folder "

        /// <summary>建立一資料夾。</summary>
        /// <returns>回傳目前指定的資料夾底下有多少個資料夾</returns>
        public static void CreateFolder(string folder)
        {
            // 確保資料夾是否存在
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
        }

        /// <summary>進行資料存檔，當該路徑原本就有資料時，此資料群將會累加到原本文件中，並且有換行的動作。</summary>
        /// <param name="path">完整路徑及副檔名</param>
        /// <param name="strFileData">加入此資料，並且加入後會進行換行</param>
        public static void AppendToFile(string path, List<string> strFileData)
        {
            AppendToFile(path, strFileData.ToArray());
        }

        /// <summary>進行資料存檔，當該路徑原本就有資料時，此資料群將會累加到原本文件中，並且有換行的動作。</summary>
        /// <param name="path">完整路徑及副檔名</param>
        /// <param name="strFileData">加入此資料，並且加入後會進行換行</param>
        public static void AppendToFile(string path, string[] strFileData)
        {
            if (strFileData != null)
            {
                lock (g_objDiskLock)
                {
                    StringBuilder clsStringBuilder = new StringBuilder();
                    foreach (string strPerLine in strFileData)
                    {
                        clsStringBuilder.AppendLine(strPerLine);
                    }
                    using (StreamWriter clsStreamWriter = new StreamWriter(path, true, Encoding.Default))
                    {
                        clsStreamWriter.Write(clsStringBuilder.ToString());
                        clsStreamWriter.Close();
                    }
                    clsStringBuilder.Clear();
                    clsStringBuilder = null;
                }
            }
        }

        /// <summary>進行資料存檔，當該路徑原本就有資料時，此資料群將會累加到原本文件中，如要換行請將 bNewLine = true。</summary>
        /// <param name="path">完整路徑及副檔名</param>
        /// <param name="fileData">加入此資料</param>
        /// <param name="addNewLine">是否換行，預設不換行。</param>
        public static void AppendToFile(string path, string fileData, bool addNewLine = false)
        {
            if (fileData != null)
            {
                lock (g_objDiskLock)
                {
                    StringBuilder clsStringBuilder = new StringBuilder();
                    fileData = addNewLine ? (fileData + Environment.NewLine) : fileData;
                    clsStringBuilder.Append(fileData);
                    using (StreamWriter clsStreamWriter = new StreamWriter(path, true, Encoding.Default))
                    {
                        clsStreamWriter.Write(clsStringBuilder.ToString());
                        clsStreamWriter.Close();
                    }
                    clsStringBuilder.Clear();
                    clsStringBuilder = null;
                }
            }
        }

        /// <summary>單純進行資料存檔。
        /// <para><b>注意！如果該路徑原本就存在，將會蓋掉原本資料!!</b></para>
        /// </summary>
        /// <param name="path">完整路徑及副檔名。</param>
        /// <param name="strFileDatas">資料內容。</param>
        /// <param name="eEncode">編碼格式，預設為 Encode.Default 。</param>
        public static void WriteFile(string path, string strFileDatas, Encode eEncode = Encode.Default)
        {
            WriteFile(path, new string[] { strFileDatas }, eEncode);
        }

        /// <summary>單純進行資料存檔。
        /// <para><b>注意！如果該路徑原本就存在，將會蓋掉原本資料!!</b></para>
        /// </summary>
        /// <param name="path">完整路徑及副檔名。</param>
        /// <param name="strFileDatas">資料內容。</param>
        /// <param name="eEncode">編碼格式，預設為 Encode.Default 。</param>
        public static void WriteFile(string path, List<string> strFileDatas, Encode eEncode = Encode.Default)
        {
            WriteFile(path, strFileDatas.ToArray(), eEncode);
        }

        /// <summary>單純進行資料存檔。
        /// <para><b>注意！如果該路徑原本就存在，將會蓋掉原本資料!!</b></para>
        /// </summary>
        /// <param name="path">完整路徑及副檔名。</param>
        /// <param name="fileDatas">資料內容。</param>
        /// <param name="eEncode">編碼格式，預設為 Encode.Default 。</param>
        public static void WriteFile(string path, string[] fileDatas, Encode eEncode = Encode.Default)
        {
            if (fileDatas != null)
            {
                Encoding eEncodeing;
                switch (eEncode)
                {
                    case Encode.ASCII:
                        eEncodeing = Encoding.ASCII;
                        break;
                    case Encode.BigEndianUnicode:
                        eEncodeing = Encoding.BigEndianUnicode;
                        break;
                    case Encode.Default:
                        eEncodeing = Encoding.Default;
                        break;
                    case Encode.Unicode:
                        eEncodeing = Encoding.Unicode;
                        break;
                    case Encode.UTF32:
                        eEncodeing = Encoding.UTF32;
                        break;
                    case Encode.UTF7:
                        eEncodeing = Encoding.UTF7;
                        break;
                    case Encode.UTF8:
                        eEncodeing = Encoding.UTF8;
                        break;
                    default:
                        eEncodeing = Encoding.UTF8;
                        break;
                }

                lock (g_objDiskLock)
                {
                    StringBuilder clsStringBuilder = new StringBuilder();
                    foreach (string strPerLine in fileDatas)
                    {
                        clsStringBuilder.AppendLine(strPerLine);
                    }
                    using (StreamWriter clsStreamWriter = new StreamWriter(path, false, eEncodeing))
                    {
                        clsStreamWriter.Write(clsStringBuilder.ToString());
                        clsStreamWriter.Close();
                    }
                    clsStringBuilder.Clear();
                    clsStringBuilder = null;

                    // 之前的做法如下，有時候會突然把檔案給咬住，即使程式關閉了也會無法釋放掉，所以換成上面的寫法。
                    // 此段註解務必留下。
                    //File.WriteAllLines(path, strFileDatas, e);
                }
            }
        }

        /// <summary>直接讀取資料檔案。再以陣列方式回傳檔案內容。</summary>
        /// <param name="path">完整路徑及副檔名。</param>
        /// <returns>回傳檔案內容，以String[]的方式承接。</returns>
        public static String[] ReadFile(string path)
        {
            lock (g_objDiskLock)
            {
                FileStream clsFileStream = null;
                StreamReader clsStreamReader = null;
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        clsFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        {
                            clsStreamReader = new StreamReader(clsFileStream, Encoding.Default);
                            List<string> strContents = new List<string>();
                            while (!clsStreamReader.EndOfStream)
                            {
                                strContents.Add(clsStreamReader.ReadLine());
                            }
                            clsStreamReader.Close();
                            clsFileStream.Close();
                            return strContents.ToArray();
                        }
                    }
                    else return null;
                }
                catch (Exception ex)
                {
                    ErrorProcessor.Record(ex);
                    if (clsStreamReader != null) clsStreamReader.Close();
                    if (clsFileStream != null) clsFileStream.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// 直接讀取資料檔案。再以陣列方式回傳檔案內容。<para></para>
        /// 更換原先使用 File.ReadAllLine() 的寫法，<para></para>
        /// 由於 ADJ 專案遇到神奇的檔案被鎖住的事件，造成無法對檔案的內容讀取，<para></para>
        /// 故改以此非同步串流讀取的方式讀取檔案，能夠越過檔案被鎖住的狀況。<para></para>
        /// 2014.04.15 by Alex 。<para></para>
        /// </summary>
        /// <param name="path">完整路徑及副檔名。</param>
        /// <param name="encoding"> Encoding 。</param>
        /// <returns>回傳檔案內容，以String[]的方式承接。</returns>
        public static String[] ReadFile(string path, Encoding encoding)
        {
            lock (g_objDiskLock)
            {
                FileStream clsFileStream = null;
                StreamReader clsStreamReader = null;
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        clsFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        {
                            clsStreamReader = new StreamReader(clsFileStream, encoding);
                            List<string> strContents = new List<string>();
                            while (!clsStreamReader.EndOfStream)
                            {
                                strContents.Add(clsStreamReader.ReadLine());
                            }
                            clsStreamReader.Close();
                            clsFileStream.Close();
                            return strContents.ToArray();
                        }
                    }
                    else return null;
                }
                catch (Exception ex)
                {
                    ErrorProcessor.Record(ex);
                    if (clsStreamReader != null) clsStreamReader.Close();
                    if (clsFileStream != null) clsFileStream.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// 單純進行資料存檔並加密。<para></para>
        /// <para><b>注意！如果該路徑原本就存在，將會蓋掉原本資料!!</b></para>
        /// </summary>
        /// <param name="path">完整路徑及副檔名。</param>
        /// <param name="strFileDatas">資料內容。</param>
        /// <param name="encode">編碼格式 (預設值為： Encode.Default ) 。</param>
        public static void WriteSecurityFile(string path, List<string> strFileDatas, Encode encode = Encode.Default)
        {
            WriteSecurityFile(path, strFileDatas.ToArray(), encode);
        }

        /// <summary>
        /// 單純進行資料存檔並加密。<para></para>
        /// <para><b>注意！如果該路徑原本就存在，將會蓋掉原本資料!!</b></para>
        /// </summary>
        /// <param name="path">完整路徑及副檔名。</param>
        /// <param name="strFileDatas">資料內容。</param>
        /// <param name="encode">編碼格式 (預設值為： Encode.Default ) 。</param>
        public static void WriteSecurityFile(string path, string[] strFileDatas, Encode encode = Encode.Default)
        {
            if (strFileDatas != null)
            {
                Encoding e;
                switch (encode)
                {
                    case Encode.ASCII:
                        e = Encoding.ASCII;
                        break;
                    case Encode.BigEndianUnicode:
                        e = Encoding.BigEndianUnicode;
                        break;
                    case Encode.Default:
                        e = Encoding.Default;
                        break;
                    case Encode.Unicode:
                        e = Encoding.Unicode;
                        break;
                    case Encode.UTF32:
                        e = Encoding.UTF32;
                        break;
                    case Encode.UTF7:
                        e = Encoding.UTF7;
                        break;
                    case Encode.UTF8:
                        e = Encoding.UTF8;
                        break;
                    default:
                        e = Encoding.UTF8;
                        break;
                }

                lock (g_objDiskLock)
                {
                    StringBuilder clsStringBuilder = new StringBuilder();
                    foreach (string strPerLine in strFileDatas)
                    {
                        clsStringBuilder.AppendLine(strPerLine);
                    }
                    using (StreamWriter clsStreamWriter = new StreamWriter(path, false, e))
                    {
                        clsStreamWriter.Write(clsStringBuilder.ToString());
                        clsStreamWriter.Close();
                    }
                    clsStringBuilder.Clear();
                    clsStringBuilder = null;
                    FileSecurity.EncryptFile(path);
                }
            }
        }

        /// <summary>
        /// 直接讀取加密資料檔案。再以陣列方式回傳檔案內容。<para></para>
        /// </summary>
        /// <param name="path">完整路徑及副檔名。</param>
        /// <returns>回傳檔案內容，以String[]的方式承接。</returns>
        public static String[] ReadSecurityFile(string path)
        {
            lock (g_objDiskLock)
            {
                FileStream clsFileStream = null;
                StreamReader clsStreamReader = null;

                try
                {
                    if (FileSecurity.IsEncryptFile(path))
                    {
                        string strTempFile = Delta.IO.Path.GetTempFile();

                        System.IO.File.Copy(path, strTempFile, true);
                        FileSecurity.DecryptFile(strTempFile);

                        List<string> strContents = new List<string>();
                        using (clsFileStream = new FileStream(strTempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (clsStreamReader = new StreamReader(clsFileStream, Encoding.Default))
                            {
                                while (!clsStreamReader.EndOfStream)
                                {
                                    strContents.Add(clsStreamReader.ReadLine());
                                }
                            }
                        }
                        File.Delete(strTempFile);
                        return strContents.ToArray();
                    }
                    else return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ReadFile] - " + " : " + ex.Message);
                    if (clsStreamReader != null) clsStreamReader.Close();
                    if (clsFileStream != null) clsFileStream.Close();
                    return null;
                }
            }
        }

        /// <summary>刪除一資料夾或檔案。</summary>
        /// <returns>回傳是否執行成功。</returns>
        public static void Delete(string path)
        {
            lock (g_objDiskLock)
            {
                // 確保資料夾是否存在
                if (System.IO.Directory.Exists(path))
                {
                    File.DeleteFilesByDate(path, DateTime.Now, DeleteDateType.Before, true, true);
                }
                else if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
        }

        /// <summary>移動資料夾位置 。
        /// <para>移動相同路徑可達到更名的效果 。</para></summary>
        /// <param name="sourceFolder">資料夾來源路徑 。</param>
        /// <param name="targetPath">資料夾目標路徑 。</param>
        /// <returns>XErrorCode 。</returns>
        public static void MoveFolder(string sourceFolder, string targetPath)
        {
            lock (g_objDiskLock)
            {
                // 確保資料夾是否存在
                if (System.IO.Directory.Exists(sourceFolder))
                {
                    System.IO.Directory.Move(sourceFolder, targetPath);
                }
                else
                {
                    throw new Exception(sourceFolder + "資料夾不存在。");
                }
            }
        }

        #endregion

        #region " Methods - Find File/Folder "

        /// <summary>指定資料夾的位置，去抓取底下有多少個資料夾，並且獲取該名稱。</summary>
        /// <param name="folder">資料夾路徑。</param>
        /// <param name="extension">指定搜尋的附檔名。</param>
        /// <returns>回傳目前指定的資料夾底下有多少個資料夾。</returns>
        public static List<string> GetFilesNameFromFolder(string folder, string extension = "*.*")
        {
            List<string> subFolders = new List<string>();			// Create return data

            // 確保資料夾是否存在
            if (!System.IO.Directory.Exists(folder))
            {
                return subFolders;   //為避免搜尋的資料夾權限若是唯讀狀態時，若再另建立新資料夾會造成系統錯誤之情況
            }

            // 設定要讀取的資料夾
            DirectoryInfo target = new DirectoryInfo(folder);

            // 取得目標底下的資料夾內的資料夾名稱  GetDirectories
            foreach (FileInfo fileInfo in target.GetFiles(extension))
            {
                if (fileInfo.Name != "")
                {
                    subFolders.Add(fileInfo.Name);
                }
            }
            return subFolders;
        }

        /// <summary>取得路徑底下所有檔案路徑。</summary>
        /// <param name="folder">資料夾路徑。</param>
        /// <param name="extension">指定搜尋的附檔名。</param>
        /// <returns>回傳檔案路徑列表。</returns>
        public static List<string> GetAllFilesPathFromFolder(string folder, string extension = "*.*")
        {
            List<string> allPathes = new List<string>();			// Create return data

            // 確保資料夾是否存在
            if (!System.IO.Directory.Exists(folder))
            {
                return allPathes;   //為避免搜尋的資料夾權限若是唯讀狀態時，若再另建立新資料夾會造成系統錯誤之情況
            }

            // 設定要讀取的資料夾
            DirectoryInfo target = new DirectoryInfo(folder);

            // 取得目標底下的資料夾內的資料檔案名稱
            foreach (FileInfo fileInfo in target.GetFiles(extension))
            {
                if (fileInfo.Name != "") allPathes.Add(folder + "\\" + fileInfo.Name);
            }

            // 取得目標底下的資料夾內的資料檔案名稱
            List<string> folderNames = GetFoldersNameFromFolder(folder);
            foreach (string folderName in folderNames)
            {
                allPathes.AddRange(GetAllFilesPathFromFolder(folder + "\\" + folderName, extension));
            }
            return allPathes;
        }

        /// <summary>回傳指定資料夾底下的檔案，並包含路徑。</summary>
        /// <param name="folder">指定搜尋的資料夾。</param>
        /// <param name="extension">指定搜尋的附檔名。</param>
        /// <returns></returns>
        public static List<string> GetFilesPathFromFolder(string folder, string extension = "*.*")
        {
            List<string> subFolderFiles = new List<string>();			// Create return data

            // 確保資料夾是否存在
            if (!System.IO.Directory.Exists(folder))
            {
                return subFolderFiles;
            }

            // 設定要讀取的資料夾
            DirectoryInfo directoryInfo = new DirectoryInfo(folder);

            // 取得目標底下的資料夾內的資料檔案名稱
            foreach (FileInfo fileInfo in directoryInfo.GetFiles(extension))
            {
                if (fileInfo.Name != "")
                {
                    subFolderFiles.Add(folder + "\\" + fileInfo.Name);
                }
            }
            return subFolderFiles;
        }

        /// <summary>指定資料夾的位置，去抓取底下有多少個資料夾，並且獲取該名稱。</summary>
        /// <param name="folder">資料夾路徑。</param>
        /// <returns>回傳目前指定的資料夾底下有多少個資料夾。</returns>
        public static List<string> GetFoldersNameFromFolder(string folder)
        {
            List<string> subFolders = new List<string>();			// Create return data

            // 確保資料夾是否存在
            if (!System.IO.Directory.Exists(folder))
            {
                return subFolders;
            }

            // 設定要讀取的資料夾
            DirectoryInfo target = new DirectoryInfo(folder);

            // 取得目標底下的資料夾內的資料夾名稱  GetDirectories
            foreach (DirectoryInfo recipe in target.GetDirectories())
            {
                if (recipe.Name != "")
                {
                    subFolders.Add(recipe.Name);
                }
            }
            return subFolders;
        }

        /// <summary>取得，應用程式啟動起始路徑。</summary>
        public static string ApplicationStartupPath
        {
            get { return File.application_StartupPath; }
        }

        private static readonly string application_StartupPath = System.AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>指定的檔案是否存在，利用這個功能，使得其他Class不需要using就能使用Files的功能。</summary>
        /// <param name="path">檔案路徑。</param>
        /// <returns>回傳Bool值，檔案是否存在。</returns>
        public static bool IsExist_File(ref string path)
        {
            bool isExist = System.IO.File.Exists(path);
            char[] dir = new char[2] { '\\', '/' };
            if (path.IndexOfAny(dir) < 0 || !isExist)
            {
                string anotherPath = ("\\" + path).Replace(@"\\", @"\");

                // 如果找不到指定的檔案，那就多加一層判斷主程式端的位置。
                if (anotherPath.Length > 1 && anotherPath.Substring(1, 1) == "\\")
                    anotherPath = application_StartupPath + anotherPath.Substring(1, anotherPath.Length - 1);
                else
                    anotherPath = application_StartupPath + anotherPath;

                isExist = System.IO.File.Exists(anotherPath);
                if (isExist) path = anotherPath;
            }
            return isExist;
        }

        /// <summary>指定的檔案是否存在，利用這個功能，使得其他Class不需要using就能使用Files的功能。</summary>
        /// <param name="path">檔案路徑。</param>
        /// <returns>回傳Bool值，檔案是否存在。</returns>
        public static bool IsExist_File(string path)
        {
            bool isExist = System.IO.File.Exists(path);
            if (!isExist)
            {
                string anotherPath = ("\\" + path).Replace(@"\\", @"\");

                // 如果找不到指定的檔案，那就多加一層判斷主程式端的位置。
                if (anotherPath.Length > 1 && anotherPath.Substring(1, 1) == "\\")
                    anotherPath = application_StartupPath + anotherPath.Substring(1, anotherPath.Length - 1);
                else
                    anotherPath = application_StartupPath + anotherPath;

                isExist = System.IO.File.Exists(anotherPath);
                if (isExist) path = anotherPath;
            }
            return isExist;
        }

        /// <summary>資料夾是否存在。</summary>
        /// <param name="path">資料夾路徑。</param>
        /// <returns>存在則回傳 True 。</returns>
        public static bool IsExist_Folder(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        #endregion

        #region " Methods - Application Function "

        /// <summary>依照日期時間刪除檔案功能。
        /// <para><b>利用此函數砍掉的檔案將不會放置於資源回收筒，所以請小心使用!!</b></para>
        /// </summary>
        /// <remarks>建議呼叫前請自行確認資料夾是否存在，否則一旦進入之後發現不存在，會另外回報錯誤，這部份請自行考量回報機制。</remarks>
        /// <param name="folderPath">給予主目錄來進行刪除檔案的動作。</param>
        /// <param name="tTime">參考的時間點。</param>
        /// <param name="eDeleteType">要刪除的檔案是在參考點往前的時間或往後的時間。</param>
        /// <param name="bNeedDeleteEmptyFolder">如果資料夾內部檔案已經清空了，是否還要刪除資料夾。</param>
        /// <param name="bSearchAllSubfolders">是否要搜索資料夾內部更多的資料夾。</param>
        /// <param name="extension">副檔名格式。</param>
        /// <example>
        /// <code>
        /// private void Example()
        /// {
        ///		// 時間的設定可以利用以下方式，即可取得上一個月的時間點。
        ///		DateTime tTime = DateTime.Now.AddMonths(-1);  
        ///		
        ///		// 也可以取得往前20天的時間點。
        ///		DateTime tTime2 = DateTime.Now.AddDays(-20);
        ///		
        ///		// 刪除 20 天前的檔案
        ///		DeleteFilesByDate(@"D:/Test",tTime2,DeleteDateType.Before);
        ///	}
        /// </code>
        /// </example>
        public static void DeleteFilesByDate(string folderPath, DateTime tTime, DeleteDateType eDeleteType,
                                                                bool bNeedDeleteEmptyFolder = false, bool bSearchAllSubfolders = false,
                                                                string extension = "*.*")
        {
            // 先確認資料夾是否存在
            if (IsExist_Folder(folderPath))
            {
                List<string> strFiles = GetFilesPathFromFolder(folderPath, extension);
                foreach (string strFile in strFiles)
                {
                    DateTime tFileTime = System.IO.File.GetLastWriteTime(strFile);
                    switch (eDeleteType)
                    {
                        case DeleteDateType.After:
                            if (tFileTime.Subtract(tTime).TotalMilliseconds >= 0)
                            {
                                File.Delete(strFile);
                            }
                            break;
                        case DeleteDateType.Before:
                            if (tFileTime.Subtract(tTime).TotalMilliseconds <= 0)
                            {
                                File.Delete(strFile);
                            }
                            break;
                    }
                }

                // 判斷是否要做更多的資料夾，如果是True代表這個資料夾內部的所有資料夾都要看!
                if (bSearchAllSubfolders)
                {
                    List<string> folders = GetFoldersNameFromFolder(folderPath);
                    foreach (string folder in folders)
                    {
                        DeleteFilesByDate(folderPath + "\\" + folder, tTime, eDeleteType, bNeedDeleteEmptyFolder, bSearchAllSubfolders, extension);
                    }
                }

                // 最後進行確認，如果資料夾內部無資料，就砍資料夾
                if (bNeedDeleteEmptyFolder)
                {
                    strFiles = GetAllFilesPathFromFolder(folderPath);
                    List<string> folders = GetFoldersNameFromFolder(folderPath);
                    if ((strFiles.Count + folders.Count) == 0)
                    {
                        System.IO.Directory.Delete(folderPath);
                    }
                }
            }
            else
            {
                ErrorProcessor.Record(new Exception(" of folder isn't exist."));
            }
        }

        /// <summary>
        /// 砍檔的呼叫指令。(利用此函數砍掉的檔案將不會放置於資源回收筒，所以請小心使用!!)<para></para>
        /// 依照設定日期刪除資料夾，可設定日期 之前 或 之後以及資料夾時間模式 如 資料夾建立日期、資料夾最後存取日期、資料夾最後寫入日期。<para></para>
        /// 並可設定是否參考子資料夾，如果子資料夾符合設定時間區段，也刪除此資料夾(僅 此資料夾下的子資料夾，子資料夾下的子資料夾便不再判斷)。
        /// </summary>
        /// <param name="folderPath">欲刪除資料夾路徑。</param>
        /// <param name="tTime">參考的時間點。</param>
        /// <param name="eFolderTimeType">資料夾時間模式。</param>
        /// <param name="eDeleteType">要刪除的資料夾是在參考點往前的時間或往後的時間。</param>
        /// <param name="bRefSubFolder">是否要也參考資料夾下的子資料夾。。</param>
        /// <remarks>僅 此資料夾下的子資料夾，子資料夾下的子資料夾便不再判斷。</remarks>
        public static void DeleteFolderByDate(string folderPath, DateTime tTime, FolderTimeType eFolderTimeType, DeleteDateType eDeleteType,
                                                                bool bRefSubFolder = false)
        {
            if (File.IsExist_Folder(folderPath))
            {
                //取得父資料夾時間
                DateTime tParentFolderTime;
                switch (eFolderTimeType)
                {
                    case FolderTimeType.CreateTime:
                        tParentFolderTime = System.IO.Directory.GetCreationTime(@folderPath);
                        break;
                    case FolderTimeType.LastAccessTime:
                        tParentFolderTime = System.IO.Directory.GetLastAccessTime(@folderPath);
                        break;
                    default:
                    case FolderTimeType.LastWriteTime:
                        tParentFolderTime = System.IO.Directory.GetLastWriteTime(@folderPath);
                        break;
                }
                //找尋子資料夾
                if (bRefSubFolder)
                {
                    //取得子資料夾時間
                    foreach (string strFName in System.IO.Directory.GetFileSystemEntries(folderPath))
                    {
                        //判斷是否是資料夾
                        if (File.IsExist_Folder(strFName))
                        {
                            DateTime tFolderTime;
                            switch (eFolderTimeType)
                            {
                                case FolderTimeType.CreateTime:
                                    tFolderTime = System.IO.Directory.GetCreationTime(@strFName);
                                    break;
                                case FolderTimeType.LastAccessTime:
                                    tFolderTime = System.IO.Directory.GetLastAccessTime(@strFName);
                                    break;
                                default:
                                case FolderTimeType.LastWriteTime:
                                    tFolderTime = System.IO.Directory.GetLastWriteTime(@strFName);
                                    break;
                            }

                            switch (eDeleteType)
                            {
                                case DeleteDateType.Before:
                                    if (DateTime.Compare(tFolderTime, tParentFolderTime) <= 0)
                                        tParentFolderTime = tFolderTime;
                                    break;
                                case DeleteDateType.After:
                                    if (DateTime.Compare(tFolderTime, tParentFolderTime) >= 0)
                                    {
                                        tParentFolderTime = tFolderTime;
                                    }
                                    break;
                            }
                        }
                    }
                }
                //刪除資料夾
                switch (eDeleteType)
                {
                    case DeleteDateType.Before:
                        if (tParentFolderTime.Subtract(tTime).TotalMilliseconds <= 0)
                        {
                            System.IO.Directory.Delete(folderPath, true);
                        }
                        break;
                    case DeleteDateType.After:
                        if (tParentFolderTime.Subtract(tTime).TotalMilliseconds >= 0)
                        {
                            System.IO.Directory.Delete(folderPath, true);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 取得資料夾內容大小。
        /// </summary>
        /// <param name="folderPath">資料夾路徑。</param>
        /// <param name="eSizeUnitType">回傳檔案大小單位。</param>
        /// <returns>回傳資料夾內容大小。</returns>
        public static long GetFolderSize(string folderPath, FileSizeUnitType eSizeUnitType = FileSizeUnitType.MB)
        {
            long dDataSize = 0;
            if (System.IO.Directory.Exists(folderPath))
            {
                foreach (string strFName in System.IO.Directory.GetFileSystemEntries(folderPath))
                {
                    //判斷是檔案還是資料夾,是資料夾的話要一直找下去,直到找不到其他資料夾為止
                    if (System.IO.File.Exists(strFName))
                    {
                        FileInfo fileInfo = new FileInfo(strFName);
                        dDataSize = fileInfo.Length + dDataSize;
                    }
                    else
                    {
                        dDataSize = GetFolderSize(@strFName, FileSizeUnitType.Byte) + dDataSize;
                    }
                }
            }
            switch (eSizeUnitType)
            {
                case FileSizeUnitType.KB:
                    dDataSize = dDataSize >> 10;
                    break;
                default:
                case FileSizeUnitType.MB:
                    dDataSize = dDataSize >> 20;
                    break;
                case FileSizeUnitType.GB:
                    dDataSize = dDataSize >> 30;
                    break;
                case FileSizeUnitType.Byte:
                    break;
            }
            return dDataSize;
        }

        /// <summary>
        /// 指定來源和目標資料夾的位置，複製來源資料夾所有檔案至目標資料夾。
        /// </summary>
        /// <param name="strSourcePath">來源資料夾的位置。</param>
        /// <param name="strTargetPath">目標資料夾的位置。</param>
        /// <param name="strFilters">指定副檔名 如 "*.jpeg,*.jpg"。</param>
        /// <param name="bDeepClone">是否深層複製。</param>
        /// <remarks>深層複製 是指複製來源底下所有檔案，淺層複製 是指只複製來源內容下檔案，不另外往下資料夾做複製。</remarks>
        public static void CopyDir(string strSourcePath, string strTargetPath, string strFilters = "*.*", bool bDeepClone = true)
        {
            if (File.IsExist_Folder(@strSourcePath))
            {
                string strFileName;
                string strDeepFileName;
                string strDeepSourcePath;
                string strDeepTargetPath;
                //檔案
                foreach (string strFile in System.IO.Directory.GetFiles(@strSourcePath).Where(s => strFilters.Contains(System.IO.Path.GetExtension(s).ToLower())))
                {
                    strFileName = System.IO.Path.GetFileName(strFile);
                    string[] folders = System.IO.Directory.GetDirectories(@strSourcePath);
                    if (bDeepClone && folders.Length > 0)
                    {
                        foreach (string folder in folders)
                        {
                            strDeepFileName = System.IO.Path.GetFileName(folder);
                            strDeepSourcePath = System.IO.Path.Combine(@strSourcePath, strDeepFileName);
                            strDeepTargetPath = System.IO.Path.Combine(@strTargetPath, strDeepFileName);
                            System.IO.Directory.CreateDirectory(@strDeepTargetPath);
                            //遞回
                            CopyDir(@strDeepSourcePath, @strDeepTargetPath, strFilters, bDeepClone);
                        }
                    }
                    //複製檔案
                    string strDestFile = System.IO.Path.Combine(strTargetPath, strFileName);
                    System.IO.File.Copy(@strFile, strDestFile, true);
                }
            }
            else
            {
                ErrorProcessor.Record(new Exception("複製資料夾來源不存在。"));
            }
        }

        /// <summary>指定來源和目標資料夾的位置，複製來源資料夾所有檔案至目標資料夾。。</summary>
        /// <param name="strSourcePath">來源資料夾。</param>
        /// <param name="strTargetPath">目標資料夾。</param>
        /// <returns>回傳是否成功，成功為 True ，失敗為 False 。</returns>
        public static bool CopyDir(string strSourcePath, string strTargetPath)
        {
            try
            {
                #region 判斷來源資料夾是否存在:不存在return false
                if (!System.IO.Directory.Exists(strSourcePath))
                {
                    //MessageBox.Show("資料夾不存在");
                    return false;
                }
                #endregion

                #region 判斷目的資料夾是否存在:若不存在，建立一個@
                if (!System.IO.Directory.Exists(strTargetPath))
                {
                    System.IO.Directory.CreateDirectory(strTargetPath);
                }
                #endregion

                #region 目的路徑最後若沒有"\\"，要補上
                if (strTargetPath[strTargetPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                {
                    strTargetPath = strTargetPath + System.IO.Path.DirectorySeparatorChar;
                }
                #endregion

                #region 複製檔案到指定路徑
                string[] strFileList = System.IO.Directory.GetFileSystemEntries(strSourcePath); // 取得來源資料夾內所有檔案的路徑
                foreach (string strFile in strFileList)
                {
                    // 若目前檔案路徑為資料夾，要再往下層搜尋檔案並複製
                    if (System.IO.Directory.Exists(strFile))
                    {
                        CopyDir(strFile, strTargetPath + System.IO.Path.GetFileName(strFile)); // (遞迴-往子資料夾處理)
                    }
                    else
                    {
                        System.IO.File.Copy(strFile, strTargetPath + System.IO.Path.GetFileName(strFile), true); // 複製檔案到指定路徑
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrorProcessor.Record(ex);
                return false;
            }

            return true;
        }

        #endregion

    }

    /// <summary>用於檔案加解密(DEC加解密) 只要是檔案都可以加密解密 (文字、圖片 ... )。</summary>
    /// <example>
    /// <code>
    ///private void Example()
    ///{
    ///    // =====================================
    ///    // 檔案加密 
    ///    // =====================================
    ///    // 先寫出一份檔案
    ///    string filename = @"D:\MyData.txt";
    ///    StreamWriter streamWrite = new StreamWriter(filename, false, Encoding.Default);
    ///    List<string> stringData = new List<string>();
    ///
    ///    stringData.Add("這是測試");
    ///    stringData.Add("你要加密的檔案(文字檔)。");
    ///
    ///    foreach (string strLine in stringData)
    ///    {
    ///        streamWrite.WriteLine(strLine);
    ///    }
    ///    streamWrite.Close();
    ///
    ///    // 你可以把硬碟的指定檔案加密後存到另外一個檔案去
    ///    FileSecurity.EncryptFile(filename, @"D:\test.tmp");
    ///    // 或者把原本的檔案讀進來並且加密後存回去
    ///    FileSecurity.EncryptFile(filename);
    ///
    ///    // =====================================
    ///    // 檔案解密 
    ///    // =====================================
    ///
    ///    // 如果有使用未加識別碼的編碼檔案可以先加上識別碼再進行解碼 (過時)
    ///    FileSecurity.ConvertToNewEncryptFile(filename);
    ///    // 檔案解碼 來源一定要是加密檔案，否則不予處理
    ///    FileSecurity.DecryptFile(filename);
    ///    // 檔案解碼 也可以寫到其他的檔案去
    ///    FileSecurity.DecryptFile(filename, @"D:\MyData.txt");
    ///}
    /// 
    /// </example>
    public class FileSecurity
    {

        /// <summary> 加密鑰匙，至少要八個字元才可以。 </summary>
        private static string key = "ai.ATLED";              // DELTA.IABG

        /// <summary> 對稱演算法的初始化向量，至少要八個字元才可以。</summary>
        private static string initialVector = "delta.IA";    // DELTA.IABG

        private static byte[] password = new byte[] { 0x19, 0x86, 0x03, 0x16 };

        /// <summary>設置金鑰。</summary>
        /// <param name="key">金鑰字元</param>
        public static void SetKey(string key)
        {
            // 要8個字元才能正常運作。
            if (key.Length == 8) FileSecurity.key = key;
        }

        /// <summary>設置初始向量。</summary>
        /// <param name="initialVector">初始向量。</param>
        public static void SetInitialVector(string initialVector)
        {
            // 要8個字元才能正常運作。
            if (initialVector.Length == 8) FileSecurity.initialVector = initialVector;
        }

        /// <summary> 對檔案進行加密動作。</summary>
        /// <param name="srcFilePath">來源檔案名稱</param>
        /// <param name="encryptFilePath">加密檔案名稱</param>
        /// <returns>true:加密成功 false:加密失敗或者此檔案已經加密過了</returns>
        public static void EncryptFile(string srcFilePath, string encryptFilePath)
        {
            lock (File.DiskLock)
            {
                if (string.IsNullOrEmpty(srcFilePath) || string.IsNullOrEmpty(encryptFilePath))
                {
                    throw new Exception("Encrypt file is empty.");
                }
                if (!System.IO.File.Exists(srcFilePath))
                {
                    throw new Exception("Encrypt file isn't exist.");
                }
                if (IsEncryptFile(srcFilePath))
                {
                    throw new Exception("The Kind of file isn't Encrypt type.");
                }

                DESCryptoServiceProvider desService = new DESCryptoServiceProvider();
                byte[] byteKey = Encoding.ASCII.GetBytes(key);
                byte[] byteVector = Encoding.ASCII.GetBytes(initialVector);

                desService.Key = byteKey;
                desService.IV = byteVector;


                using (FileStream srcStream = new FileStream(srcFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream tagStream = new FileStream(encryptFilePath, FileMode.Create, FileAccess.Write))
                    {
                        //檔案加密
                        byte[] byteDataArray = new byte[srcStream.Length];
                        srcStream.Read(byteDataArray, 0, byteDataArray.Length);

                        using (CryptoStream clsStream = new CryptoStream(tagStream, desService.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            clsStream.Write(byteDataArray, 0, byteDataArray.Length);
                            clsStream.FlushFinalBlock();
                        }
                    }
                }

                // 加入 Password 識別碼
                AddPassword(encryptFilePath);
            }
        }

        /// <summary> 對檔案進行加密動作 </summary>
        /// <param name="filePath">檔案名稱</param>
        /// <returns>ture:加密成功  false:加密失敗或者此檔案已經是加密的檔案了。</returns>
        public static void EncryptFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("Encrypt file is empty.");
            }
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Encrypt file isn't exist.");
            }
            if (IsEncryptFile(filePath))
            {
                throw new Exception("The Kind of file isn't Encrypt type.");
            }

            // 暫存檔路徑
            string tmpPath = Delta.IO.Path.GetTempFile();

            // 複製成站存檔
            System.IO.File.Copy(filePath, tmpPath, true);
            // 將屬性設為隱藏
            System.IO.File.SetAttributes(tmpPath, FileAttributes.Hidden);
            // 檔案加密
            EncryptFile(tmpPath, filePath);
            // 刪除暫存檔
            System.IO.File.Delete(tmpPath);
        }

        /// <summary> 檔案解碼，要注意你用的 Key 與 IV 是否正確，如果不正確的話是無法解密的。</summary>
        /// <param name="encryptFilePath">加碼檔案名稱</param>
        /// <param name="decryptFilePath">解碼檔案名稱</param>
        /// <returns>true:解碼成功   false:解碼失敗、者檔案不存在或此檔案不是加密檔</returns>
        public static void DecryptFile(string encryptFilePath, string decryptFilePath)
        {
            if (string.IsNullOrEmpty(encryptFilePath) || string.IsNullOrEmpty(decryptFilePath))
            {
                throw new Exception("Encrypt file is empty.");
            }
            if (!System.IO.File.Exists(encryptFilePath))
            {
                throw new Exception("Encrypt file isn't exist.");
            }

            if (!IsEncryptFile(encryptFilePath))
            {
                throw new Exception("The Kind of file isn't Encrypt type.");
            }

            // 移除 Password
            RemovePassword(encryptFilePath);

            DESCryptoServiceProvider desService = new DESCryptoServiceProvider();
            byte[] byteKey = Encoding.ASCII.GetBytes(key);
            byte[] byteInitialVector = Encoding.ASCII.GetBytes(initialVector);

            desService.Key = byteKey;
            desService.IV = byteInitialVector;

            lock (File.DiskLock)
            {
                using (FileStream srcStream = new FileStream(encryptFilePath, FileMode.Open, FileAccess.Read))
                using (FileStream tagStream = new FileStream(decryptFilePath, FileMode.Create, FileAccess.Write))
                {

                    byte[] dataArray = new byte[srcStream.Length];
                    srcStream.Read(dataArray, 0, dataArray.Length);
                    using (CryptoStream stream = new CryptoStream(tagStream, desService.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        stream.Write(dataArray, 0, dataArray.Length);
                        stream.FlushFinalBlock();
                    }
                }
            }

            // 加回去
            AddPassword(encryptFilePath);
        }

        /// <summary> 對檔案進行解密動作。 </summary>
        /// <param name="filePath">檔案名稱</param>
        /// <returns>true:解密成功   false:解碼失敗、者檔案不存在或此檔案不是加密檔</returns>
        public static void DecryptFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("Encrypt file is empty.");
            }
            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Encrypt file isn't exist.");
            }
            if (!IsEncryptFile(filePath))
            {
                throw new Exception("The Kind of file isn't Encrypt type.");
            }

            // 暫存檔路徑
            string tmpPath = Delta.IO.Path.GetTempFile();

            // 複製成暫存檔
            System.IO.File.Copy(filePath, tmpPath, true);
            // 檔案解密
            DecryptFile(tmpPath, filePath);
            // 將屬性設為隱藏
            System.IO.File.SetAttributes(tmpPath, FileAttributes.Hidden);
            // 刪除暫存檔
            System.IO.File.Delete(tmpPath);
        }

        /// <summary>加入有加密的識別碼。</summary>
        /// <param name="filePath">檔案路徑</param>
        private static void AddPassword(string filePath)
        {
            lock (File.DiskLock)
            {
                using (FileStream srcStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    srcStream.Seek(srcStream.Length, 0);
                    srcStream.Write(password, 0, password.Length);
                }
            }
        }

        /// <summary>移除最後一行的識別碼。</summary>
        /// <param name="filePath">檔案路徑</param>
        private static void RemovePassword(string filePath)
        {
            byte[] dataArray;
            lock (File.DiskLock)
            {
                using (FileStream srcStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    dataArray = new byte[srcStream.Length];
                    srcStream.Read(dataArray, 0, dataArray.Length);
                }

                using (FileStream srcStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    srcStream.Write(dataArray, 0, dataArray.Length - 4);
                }
            }
        }

        /// <summary>判斷這個檔案是否有加密。</summary>
        /// <param name="filePath">檔案名稱</param>
        /// <returns>true:此為加密檔   false:不是加密檔或檔案不存在</returns>
        public static bool IsEncryptFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath)) return false;

            bool isEncrypt = true;
            lock (File.DiskLock)
            {
                using (FileStream srcStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] dataArray = new byte[srcStream.Length];
                    srcStream.Read(dataArray, 0, dataArray.Length);

                    if (dataArray.Count() - 4 < 0) return false;

                    for (int i = 0; i < password.Count(); i++)
                    {
                        if (dataArray[srcStream.Length - 4 + i] != password[i])
                        {
                            isEncrypt = false;
                            break;
                        }
                    }
                }
            }
            return isEncrypt;
        }

        /// <summary>將有加密但是沒有加入識別碼的加密檔案添加識別碼。</summary>
        /// <param name="filePath">檔案路徑</param>
        /// <returns>true:轉換成功 false:轉換失敗，或者本檔案本來就已經包含識別碼</returns>
        public static bool ConvertToNewEncryptFile(string filePath)
        {
            if (IsEncryptFile(filePath))
            {
                return false;
            }
            else
            {
                AddPassword(filePath);
                return true;
            }
        }

    }

    /// <summary>
    /// 用來紀錄你要得資訊內容，紀錄方式可以依照時間作切割分檔
    /// </summary>
    /// <example>
    /// <code>
    /// public void Example()
    /// {
    ///     // Example by Frank Jian.
    ///     // 寫入資料夾，檔案名稱，未來檔案會以Day, Hour or Minute分成不同檔案名稱去做存檔的動作。
    ///     XLog clsTestLog = new XLog(@"D:\要記錄的資料夾位置", "給一個檔案名稱.txt", XLog.LogRecordGroup.Minute, "Vision的記錄檔");
    ///
    ///     // 可以重新設定存檔的型態，看是要一個小時存一個檔案還是一天、一分鐘，如果選none就是不分檔案，會全部存在同一個，小心爆掉!!
    ///     clsTestLog.LogRecordType = LogRecordGroup.Hours;
    ///
    ///     // 可以設定紀錄內容的時間格式! 設定的方式有下面這幾類型。
    ///     clsTestLog.SetDateFormat();
    ///     clsTestLog.SetDateFormat("MMddHHmmss");
    ///     clsTestLog.SetDateFormat("", false, false, true, true, true, false, false);
    ///     clsTestLog.SetDateFormat("_", false, false, true, true, false, false, false);
    ///
    ///     // 要記錄單行的時候~
    ///     clsTestLog.WriteLog("單行就是直接放一個string就好");
    ///
    ///     // 要紀錄附帶自訂格式變數的字串時
    ///     int iA = 3, iB = 2;
    ///     clsTestLog.WriteLog("{0} = {1} + {2}", iA + iB, iA, iB);
    ///
    ///     // 要記錄多行的時候~ (多行的話必須是一個 Array,&#60; List > 的型式)
    ///     List &#60;string> strLog = new List &#60;string>();
    ///     strLog.Add(DateTime.Now.Millisecond.ToString());
    ///     strLog.Add(DateTime.Now.Millisecond.ToString());
    ///     strLog.Add(DateTime.Now.Date.ToString());
    ///
    ///     clsTestLog.WriteLog(strLog.ToArray());
    /// }
    /// </code>
    /// </example>
    public class Log
    {

        #region" Definities "

        /// <summary> Log 的紀錄方式，如果是以時間來分，有哪幾種型態。</summary>
        public enum LogRecordGroup
        {
            /// <summary> 不以時間做區分，全部的資料都在同一個資料夾。</summary>
            None,
            /// <summary> 一分鐘為一個檔案。</summary>
            Minute,
            /// <summary> 一小時為一個檔案。</summary>
            Hour,
            /// <summary> 一天為一個檔案。</summary>
            Day,
        }

        #endregion

        #region " Field "

        private LogRecordGroup g_eLogRecordType;
        private string g_strArrayProString;
        private string g_strLogFolder;
        private string g_strLogFileName;
        private string g_strDescription;
        private string g_strDateFormat = "yyyy/MM/dd HH:mm:ss.fff";

        #endregion

        #region " Properties "

        /// <summary>檔案型態是要分成一個小時、一分鐘、一天為一份檔案或者其他模式。</summary>
        public LogRecordGroup LogRecordType
        {
            get { return g_eLogRecordType; }
            set { g_eLogRecordType = value; }
        }

        /// <summary>檔案的根目錄。</summary>
        public string Folder
        {
            get { return g_strLogFolder; }
            set { g_strLogFolder = value; }
        }

        /// <summary>檔案預設名稱(實際檔案可能會加上時間的資訊)。</summary>
        public string FileName
        {
            get { return g_strLogFileName; }
            set { g_strLogFileName = value; }
        }

        /// <summary> 取得或設定，物件描述 </summary>
        public string Description
        {
            get { return g_strDescription; }
            set { g_strDescription = value; }
        }

        #endregion

        #region " Methods - New "

        /// <summary>建構一個Log檔案類別，未來可以快速的紀錄要記錄的文字。</summary>
        /// <param name="folder">資料夾根目錄。</param>
        /// <param name="strFileName">檔案預設名稱(實際檔案可能會加上時間的資訊)。</param>
        /// <param name="eLogRecordType">檔案型態是要分成一個小時、一分鐘、一天為一份檔案或者其他模式。</param>
        /// <param name="strDestript">關於此紀錄檔的敘述。</param>
        public Log(string folder, string strFileName, LogRecordGroup eLogRecordType, string strDestript = "")
        {
            LogFolderSetup(folder, strFileName);
            g_eLogRecordType = eLogRecordType;
            g_strDescription = strDestript;
            SetDateFormat();
        }

        #endregion

        #region " Methods - Write Log "

        /// <summary> Log的參數設定。</summary>
        /// <param name="folder">資料夾根目錄。</param>
        /// <param name="strFileName">檔案預設名稱(實際檔案可能會加上時間的資訊)。</param>
        public void LogFolderSetup(string folder, string strFileName)
        {
            folder = (folder + @"\").Replace(@"\\", @"\");    // 自動在後面的字元加入"\"的字元
            g_strLogFolder = folder;

            if (!File.IsExist_Folder(g_strLogFolder)) File.CreateFolder(g_strLogFolder);

            g_strLogFileName = strFileName;
        }

        /// <summary>撰寫一段文字到檔案。</summary>
        /// <param name="strMessage">要記錄的訊息。</param>
        public void WriteLog(string strMessage)
        {
            string[] strMessages = new string[1] { strMessage };
            WriteLog(strMessages);
        }

        /// <summary>撰寫一段文字到檔案。</summary>
        /// <param name="strFormat"></param>
        /// <param name="objPrarams"></param>
        public void WriteLog(string strFormat, params object[] objPrarams)
        {
            string[] strMessages = new string[1] { string.Format(strFormat, objPrarams) };
            WriteLog(strMessages);
        }

        /// <summary>撰寫多行文字到檔案。</summary>
        /// <param name="strMessage">要寫入的文字資訊。</param>
        public void WriteLog(string[] strMessage)
        {
            string strDateString = DateTime.Now.ToString(g_strDateFormat) + "\t";
            int iMessageLenght = strMessage.Length;
            if (iMessageLenght > 0)
            {
                strMessage[0] = strDateString + strMessage[0];

                for (int iMessageLine = 1; iMessageLine < iMessageLenght; iMessageLine++)
                {
                    strMessage[iMessageLine] = g_strArrayProString + strMessage[iMessageLine];
                }

                string strProFileName = string.Empty;
                switch (g_eLogRecordType)
                {
                    case LogRecordGroup.Day:
                        strProFileName = DateTime.Now.ToString("yyyyMMdd");
                        break;

                    case LogRecordGroup.Hour:
                        strProFileName = DateTime.Now.ToString("yyyyMMddHH");
                        break;

                    case LogRecordGroup.Minute:
                        strProFileName = DateTime.Now.ToString("yyyyMMddHHmm");
                        break;
                }

                File.AppendToFile(g_strLogFolder + strProFileName + g_strLogFileName, strMessage);
            }
        }

        /// <summary>撰寫多行文字到檔案。</summary>
        /// <param name="strMessage">要寫入的文字資訊。</param>
        public void WriteLog(List<string> strMessage)
        {
            WriteLog(strMessage.ToArray());
        }

        /// <summary>設定檔案的時間顯示格式。</summary>
        /// <param name="strDateFormat">時間格式，會用在檔名的分類上。</param>
        public void SetDateFormat(string strDateFormat = "yyyy/MM/dd HH:mm:ss.fff")
        {
            g_strDateFormat = strDateFormat;

            // 設定Array的前字串。
            int iArrayProString = (g_strDateFormat + "\t").Length;
            g_strArrayProString = string.Empty;
            for (int iIndex = 0; iIndex < iArrayProString; iIndex++)
            {
                g_strArrayProString += " ";
            }
        }

        /// <summary>設定檔案的時間顯示格式。</summary>
        /// <param name="strSpiltChar"></param>
        /// <param name="bMillisecond"></param>
        /// <param name="bSecond"></param>
        /// <param name="bMinute"></param>
        /// <param name="bHour"></param>
        /// <param name="bDay"></param>
        /// <param name="bMonth"></param>
        /// <param name="bYear"></param>
        public void SetDateFormat(string strSpiltChar, bool bMillisecond = true, bool bSecond = true, bool bMinute = true, bool bHour = true,
                                                    bool bDay = true, bool bMonth = true, bool bYear = true)
        {
            List<string> strDateFormat = new List<string>();
            if (bYear) strDateFormat.Add("yyyy");
            if (bMonth) strDateFormat.Add("MM");
            if (bDay) strDateFormat.Add("dd");
            if (bHour) strDateFormat.Add("HH");
            if (bMinute) strDateFormat.Add("mm");
            if (bSecond) strDateFormat.Add("ss");
            if (bMillisecond) strDateFormat.Add("fff");
            g_strDateFormat = String.Join(strSpiltChar, strDateFormat);
            g_strDateFormat = g_strDateFormat + "";
            SetDateFormat(g_strDateFormat);
        }

        #endregion

    }

}