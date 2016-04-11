using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Delta.IO
{

    /// <summary> 
    /// 標準系統設定檔工具，寫出的格式將為
    /// <para>[Sation]</para>
    /// <para>Key = Value</para>
    /// </summary>
    /// <example>
    /// <code>
    /// private void Example()
    /// {
    ///     // 建立時要給路徑,在此模擬寫入資料到ini檔案中。
    ///     XIni ini = new Ini(@"D:\123.ini");
    ///     ini.Write("Section1", "Key", "26");
    ///     ini.Write("Section1", "a~y", "20");
    ///     ini.Write("Section2", "a~y", "25");
    ///
    ///     // 建立時要給路徑,在此模擬讀取ini檔案中的資料。
    ///     Ini ini2 = new Ini(@"D:\123.ini");
    ///     string value = ini2.Read("Section1", "Key");
    /// }
    /// </code>
    /// </example>
    public class Ini
    {

        #region " Field "

        private string iniPath;
        private EventArgs args = new EventArgs();
  
        #endregion

        #region " Property "

        /// <summary> 取得，設定檔路徑 </summary>
        public string IniPath { get { return iniPath; } protected set { iniPath = value; } }

        #endregion

        #region " Import "

        /// <summary>
        /// 於指定Section/Name中取得StringBuilder
        /// </summary>
        /// <param name="section">章</param>
        /// <param name="key">索引名稱</param>
        /// <param name="def">預設值</param>
        /// <param name="retVal">回傳的字串</param>
        /// <param name="size">欲回傳的字串長度</param>
        /// <param name="filePath">檔案位址</param>
        /// <returns></returns>
        [DllImport("kernel32")]			// Ini 檔案的讀取
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>於指定Section/Name中取得整數。</summary>
        /// <param name="section">章</param>
        /// <param name="key">索引名稱</param>
        /// <param name="def">預設值</param>
        /// <param name="filePath">檔案位址</param>
        /// <returns></returns>
        [DllImport("kernel32")]			// Ini 檔案的讀取 (整數)
        public static extern int GetPrivateProfileInt(string section, string key, int def, string filePath);

        /// <summary>於指定Section/Key中寫入字串。</summary>
        /// <param name="section">章</param>
        /// <param name="key">Key名稱</param>
        /// <param name="val">欲寫入數據</param>
        /// <param name="filePath">檔案位址</param>
        /// <returns></returns>
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>於指定Section/Name中取得位元組。</summary>
        /// <param name="section">章</param>
        /// <param name="key">索引名稱</param>
        /// <param name="def">預設值</param>
        /// <param name="retArray">回傳陣列</param>
        /// <param name="size">欲回傳的陣列長度</param>
        /// <param name="filePath">檔案位址</param>
        /// <returns></returns>
        [DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileStringW")]
        public static extern uint GetPrivateProfileStringByByteArray(string section, string key, string def, byte[] retArray, uint size, string filePath);

        /// <summary>於指定Section/Key中取得字串。</summary>
        /// <param name="section">章</param>
        /// <param name="key">索引名稱</param>
        /// <param name="def">預設值</param>
        /// <param name="retArray">回傳陣列</param>
        /// <param name="size">欲回傳的陣列長度</param>
        /// <param name="filePath">檔案位址</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, byte[] retArray, int size, string filePath);

        /// <summary>於指定Section中取得所有字串。</summary>
        /// <param name="section">章</param>
        /// <param name="retArray">回傳陣列</param>
        /// <param name="size">欲回傳的陣列長度</param>
        /// <param name="filePath">檔案位址</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileSection(string section, byte[] retArray, int size, string filePath);

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionNames", SetLastError = true)]
        private static extern uint GetPrivateProfileSectionNames(IntPtr retVal, uint size, string filePath);

        [DllImport("kernel32.dll")]
        public static extern int WritePrivateProfileSection(string lpAppName, string lpstring, string lpFileName);

        #endregion

        #region " Events "

        /// <summary>讀取後觸發此事件。</summary>
        public event EventHandler Read;

        /// <summary>寫入後觸發此事件。</summary>
        public event EventHandler Wrote;

        #endregion

        #region " Methods - New "

        /// <summary>建構一個Ini的單元。</summary>
        /// <param name="iniPath">指定Ini檔案的路徑位置。</param>
        public Ini(string iniPath)
        {
            this.iniPath = iniPath;
        }

        #endregion

        #region " Methods - Write "

        /// <summary> 撰寫INI文件中的特定子區塊 </summary> 
        public virtual void Write(string section, string key, string strValue)
        {
            WritePrivateProfileString(section, key, strValue, iniPath);
            OnWrote();
        }

        /// <summary>撰寫INI文件中的特定子區塊 (整數)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="value">整數值</param>
        public virtual void WriteInteger(string section, string key, int value)
        {
            WritePrivateProfileString(section, key, value.ToString(), iniPath);
            OnWrote();
        }

        /// <summary>撰寫INI文件中的特定子區塊 (浮點數)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="value">浮點數值</param>
        public virtual void WriteFloat(string section, string key, float value)
        {
            WritePrivateProfileString(section, key, value.ToString(), iniPath);
            OnWrote();
        }

        /// <summary>撰寫INI文件中的特定子區塊 (浮點數)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="value">浮點數值</param>
        public virtual void WriteFloat(string section, string key, double value)
        {
            WritePrivateProfileString(section, key, value.ToString(), iniPath);
            OnWrote();
        }

        /// <summary>撰寫INI文件中的特定子區塊 (浮點數)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="value">浮點數值</param>
        public virtual void WriteDouble(string section, string key, double value)
        {
            WritePrivateProfileString(section, key, value.ToString(), iniPath);
            OnWrote();
        }

        /// <summary>撰寫INI文件中的特定子區塊 (布林值)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="value">布林值</param>
        public virtual void WriteBoolean(string section, string key, bool value)
        {
            WritePrivateProfileString(section, key, value.ToString(), iniPath);
            OnWrote();
        }

        /// <summary>撰寫INI文件中的特定子區塊 (字串)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="value">字串</param>
        public virtual void WriteString(string section, string key, string value)
        {
            Write(section, key, value);
            OnWrote();
        }

        #endregion

        #region " Methods - Read "

        /// <summary> 讀取INI文件中的特定子區塊 </summary> 
        /// <param name="section">項目名稱(如 [TypeName] )</param> 
        /// <param name="key">資料群</param> 
        /// <param name="size">字串序列的長度 (預設 500)</param> 
        /// <returns>回傳"="後面的字串</returns> 
        public virtual string ReadIni(string section, string key, int size = 500)
        {
            StringBuilder receive = new StringBuilder(size);
            int i = GetPrivateProfileString(section, key, "", receive, size, iniPath);
            OnRead();
            return receive.ToString();
        }

        /// <summary>讀取INI文件中的特定子區塊 (整數)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns>回傳"="後面的整數</returns>
        public virtual int ReadInteger(string section, string key, int defaultValue)
        {
            int result = GetPrivateProfileInt(section, key, defaultValue, iniPath);
            OnRead();
            return result;
        }

        /// <summary>讀取INI文件中的特定子區塊 (浮點數)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="defaultValue">預設值</param>
        /// <param name="size">指定讀取的Buffer數量。</param>
        /// <returns>回傳"="後面的浮點數</returns>
        public virtual float ReadFloat(string section, string key, float defaultValue, int size = 500)
        {
            float result = 0;
            StringBuilder stringBuilder = new StringBuilder(size);
            string sDefault = defaultValue + "";
            GetPrivateProfileString(section, key, sDefault, stringBuilder, size, iniPath);
            if (float.TryParse(stringBuilder.ToString(), out result))
            {
                OnRead();
            }
            else
            {
                ErrorProcessor.Record(ErrorCode.ER_DATA_TRANSFER, false);
            }
            return result;
        }

        /// <summary>讀取INI文件中的特定子區塊 (浮點數)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="defaultValue">預設值</param>
        /// <param name="size">指定讀取的Buffer數量。</param>
        /// <returns>回傳"="後面的浮點數</returns>
        public virtual float ReadFloat(string section, string key, double defaultValue, int size = 500)
        {
            float result = 0;
            StringBuilder stringBuilder = new StringBuilder(size);
            string sDefault = defaultValue + "";
            GetPrivateProfileString(section, key, sDefault, stringBuilder, size, iniPath);
            if (float.TryParse(stringBuilder.ToString(), out result))
            {
                OnRead();
            }
            else
            {
                ErrorProcessor.Record(ErrorCode.ER_DATA_TRANSFER, false);
            }
            return result;
        }

        /// <summary>讀取INI文件中的特定子區塊 (浮點數)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="defaultValue">預設值</param>
        /// <param name="size">指定讀取的Buffer數量。</param>
        /// <returns>回傳"="後面的浮點數</returns>
        public virtual double ReadDouble(string section, string key, double defaultValue, int size = 500)
        {
            double result = 0;
            StringBuilder stringBuilder = new StringBuilder(size);
            string sDefault = defaultValue + "";
            GetPrivateProfileString(section, key, sDefault, stringBuilder, size, iniPath);
            if (double.TryParse(stringBuilder.ToString(), out result))
                OnRead();
            else            
                ErrorProcessor.Record(ErrorCode.ER_DATA_TRANSFER, false);
            
            return result;
        }
        
        /// <summary>讀取INI文件中的特定子區塊 (浮點數)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="defaultValue">預設值</param>
        /// <param name="size">指定讀取的Buffer數量。</param>
        /// <returns>回傳"="後面的True/False</returns>
        public virtual bool ReadBoolean(string section, string key, bool defaultValue, int size = 500)
        {
            bool result;
            StringBuilder stringBuilder = new StringBuilder(size);
            GetPrivateProfileString(section, key, defaultValue.ToString(), stringBuilder, size, iniPath);

            if (stringBuilder.ToString().ToUpper() == "TRUE" || stringBuilder.ToString() == "1")
            {
                result = true;
            }
            else
            {
                result = false;
            }
            OnRead();
            return result;
        }

        /// <summary>讀取INI文件中的特定子區塊 (字串)</summary>
        /// <param name="section">項目名稱(如 [TypeName] )</param>
        /// <param name="key">資料群</param>
        /// <param name="defaultValue">預設值</param>
        /// <param name="size">指定讀取的Buffer數量。</param>
        /// <returns>回傳"="後面的字串</returns>
        public virtual string ReadString(string section, string key, string defaultValue, int size = 500)
        {
            StringBuilder result = new StringBuilder(size);
            GetPrivateProfileString(section, key, defaultValue, result, size, iniPath);
            OnRead();
            return result.ToString();
        }

        #endregion

        #region " Methods - Delete "

        /// <summary>刪除INI文件中的特定的子區塊</summary>
        /// <param name="section">項目名稱([XXXX])</param>
        /// <param name="key">資料群</param>
        public virtual void DeleteKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, iniPath);
            OnRead();
        }

        public virtual bool DeleteSection(string section)
        {
            if (WritePrivateProfileSection(section, null, iniPath) != 0)
                return true;
            else
                return false;
        }

        #endregion

        #region " Methods - Get "

        /// <summary>於指定Section中取得所有的Key串列。</summary>
        /// <param name="section">章節名稱。</param>
        /// <returns>回傳Ini內被指定章節當中，所有的key names。</returns>
        public List<string> GetAllKeys(string section)
        {
            return GetAllKeys(iniPath, section);
        }

        /// <summary>於指定Section中取得所有的Key串列。</summary>
        /// <param name="filePath">檔案路徑</param>
        /// <param name="section">章節名稱。</param>
        /// <returns>回傳Ini內被指定章節當中，所有的key names。</returns>
        public static List<string> GetAllKeys(string filePath, string section)
        {

            byte[] buffer = new byte[2048];

            GetPrivateProfileSection(section, buffer, 2048, filePath);
            String[] tmp = Encoding.UTF8.GetString(buffer).Trim('\0').Split('\0');

            List<string> result = new List<string>();

            foreach (String entry in tmp)
            {
                result.Add(entry.Substring(0, entry.IndexOf("=")));
            }

            return result;
        }

        /// <summary>取得所有的Section names。</summary>
        /// <returns>回傳Ini內所有的Section names。</returns>
        public string[] GetAllSectionNames()
        {
            return GetAllSectionNames(iniPath);
        }

        /// <summary>取得所有的Section names。</summary>
        /// <param name="path">檔案路徑。</param>
        /// <returns>回傳Ini內所有的Section names。</returns>
        public static string[] GetAllSectionNames(string path)
        {
            uint MAX_BUFFER = 32767;
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER);
            uint bytesReturned = GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, path);
            return IntPtrToStringArray(pReturnedString, bytesReturned);
        }

        //指標資料轉字串陣列
        private static string[] IntPtrToStringArray(IntPtr pReturnedString, uint bytesReturned)
        {
            //use of Substring below removes terminating null for split
            if (bytesReturned == 0)
            {
                Marshal.FreeCoTaskMem(pReturnedString);
                return null;
            }
            string local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned).ToString();
            Marshal.FreeCoTaskMem(pReturnedString);
            return local.Substring(0, local.Length - 1).Split('\0');
        }

        #endregion

        #region " Methods - Event Process "

        /// <summary>手動觸發已讀取的事件功能。</summary>
        protected void OnRead()
        {
            if (Read != null) Read(this, args);
        }

        /// <summary>手動觸發已寫入的事件功能。</summary>
        protected void OnWrote()
        {
            if (Wrote != null) Wrote(this, args);
        }

        #endregion

    }

    /// <summary>加密型的Ini檔案。</summary>
    public class SecurityIni : Ini , IDisposable
    {

        #region " Field "

        private EventWaitHandle encryptWaitHandle = new AutoResetEvent(false);

        #endregion

        #region " Properties "

        /// <summary>取得 加密的檔案來源/目的。</summary>
        public string SourcePath { private set; get; }

        #endregion

        #region " Methods - New "

        /// <summary>建構一個Ini的單元。</summary>
        /// <param name="iniPath">指定Ini檔案的路徑位置。</param>
        public SecurityIni(string iniPath)
            : base(iniPath)
        {
            SourcePath = iniPath;

            // 建立一個暫存檔路徑
            base.IniPath = Delta.IO.Path.GetTempFile();
            if (File.IsExist_File(iniPath))
                FileSecurity.DecryptFile(iniPath, base.IniPath);

            // Events and thread setup
            base.Wrote += Ini_Wrote;
        }

        /// <summary>解構時，檢查檔案是否存在，如果存在的話需要將檔案移除。</summary>
        ~SecurityIni()
        {
            Dispose();
        }

        /// <summary>刪除目的檔案及連結路徑。</summary>
        public void Dispose()
        {
            try
            {
                if (File.IsExist_File(base.IniPath)) File.Delete(base.IniPath);
            }
            catch (Exception ex)
            {
                ErrorProcessor.Record(ex);
            }
            finally
            {
                base.IniPath = "";
            }
        }    
    
        #endregion

        #region " Events - Ini Save or Write "

        private void Ini_Wrote(object sender, EventArgs e)
        {
            lock (this)
            {
                if (File.IsExist_File(base.IniPath))
                    FileSecurity.EncryptFile(base.IniPath, SourcePath);
            }
        }

        #endregion

    }

}
