using Delta;
using Delta.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Delta
{
    /// <summary> 此類別負責處理例外狀況的回報問題，有任何例外狀況抓到時，都應該透過此類別內的 Record(Exception ex) 來進行上報動作，或者透過字串方式來告知錯誤訊息。
    /// <para></para>例外處理這塊，主要提供三種功能讓開發者自行選擇不同情境下的處理方法。
    /// <para></para>1. 如果是 try...catch(Exception ex) 抓到的錯誤(非預期的錯誤)，請使用 ErrorProcessor.Record(Exception ex); throw;
    /// <para></para>2. 如果是預期的錯誤，則會有自定義的錯誤碼，請使用 throw ErrorProcessor.CustomException(ErrorCode.ER_SYSTEM);
    /// <para></para>3. 以上兩種分別是預期跟不可預期的錯誤處理，還有一種是單純紀錄資訊的動作，不拋錯誤，使程式可以繼續執行。
    /// <para></para>   用法如右，ErrorProcessor.Record("需要被記錄的字串")
    /// <para></para>   但是如果也需要多國語系的翻譯的話，請使用錯誤碼定義， ErrorProcessor.Record(ErrorCode.ER_SYSTEM, false)
    /// </summary>
    public static class ErrorProcessor
    {

        #region " Definition "

        private const string EXCEPTION_RECORD_CYMBOL = "#%";

        /// <summary> 錯誤回報時紀錄時間的格式。</summary>
        public const string TIME_FORMAT = "yyyy/MM/dd HH:mm:ss.fff";

        /// <summary>紀錄區間。</summary>
        public enum FileInterval
        {
            /// <summary>無。</summary>
            None,
            /// <summary>以小時為區間單位。</summary>
            Hours,
            /// <summary>以日為區間單位。</summary>
            Days,
        }

        /// <summary>錯誤處理資料的存檔模式類型。</summary>
        public enum SaveItems
        {
            /// <summary>系統預設。</summary>
            Default,
            /// <summary>立即存檔。</summary>
            Immediate,
            /// <summary>非立即存檔。</summary>
            NonImmediate
        }

        /// <summary> EventHandler的委派定義為回拋一段文字。</summary>
        public delegate void EventHandler(string strMessage);

        #endregion

        #region " Properties "

        private static Language language = new Language("ASD.General.Properties.Error");

        private static int recorderLines = 20;

        /// <summary>系統資訊被紀錄的比數上限。</summary>
        public static int RecorderLine { get { return recorderLines; } set { recorderLines = value; } }

        /// <summary> 存檔的時間間隔類型。</summary>
        public static FileInterval RecordType
        {
            set { recordType = value; }
        }
        private static FileInterval recordType = FileInterval.None;

        /// <summary> 要存檔案的資料夾根目錄。</summary>
        public static string LogFolderPath
        {
            get { return logFolderPath; }
            set { logFolderPath = value; }
        }
        private static string logFolderPath = string.Empty;

        /// <summary> 回報後整理的內容資料。</summary>
        public static List<string> LogMessage = new List<string>();

        private static object lockReport = new object();
        private static object lockReserveMessage = new object();
        private static EventArgs eventArgs = new EventArgs();

        #endregion

        #region " Methods - Record "

        /// <summary>提供一個例外狀況時的回報功能，回報資訊之後，可以將問題依照某種資料方式存放下來，使未來更容易追蹤錯誤的歷史紀錄。</summary>
        /// <param name="ex">例外狀況的資料內容。</param>
        public static void Record(Exception ex)
        {
            try
            {
                // Check exception whether was recorded.
                if (ex != null && (ex.Source != null))
                {
                    if (ex.Source.IndexOf(EXCEPTION_RECORD_CYMBOL, 0, 2) < 0)
                    {
                        lock (lockReport)
                        {
                            // 先放入時間文字，之後要繼續堆疊後面的資料。
                            string strAllMessage = DateTime.Now.ToString(TIME_FORMAT) + ", " + ex.StackTrace;

                            // 最後填入一個識別碼，這個識別碼是用來記錄說這段Exception已經被記錄下來了，之後如果再進入此函數就不會再處理。
                            ex.Source = EXCEPTION_RECORD_CYMBOL + ex.StackTrace;

                            // 將資料插入到第一個，所以第一個會是最新的訊息，最後一個是最舊的訊息。
                            LogMessage.Insert(0, strAllMessage);

                            // 如果訊息超過500個，就刪掉最舊的，以保持訊息的長度不會發散。
                            while (LogMessage.Count > recorderLines) LogMessage.RemoveAt(recorderLines);

                            // Trigger the event Recorded.
                            if (Recorded != null) Recorded(LogMessage[0]);
                        }
                    }
                }
                else if (ex.Message.Length > 0)
                {
                    Record(ex.Message);
                }
            }
            catch
            {
            }
        }

        /// <summary>提供一個例外狀況時的回報功能，回報資訊之後，可以將問題依照某種資料方式存放下來，使未來更容易追蹤錯誤的歷史紀錄。
        /// <para></para>通常使用這個函數功能的原因是因為不想拋Exception造成例外錯誤，只想記錄一些資訊。
        /// <para></para>如果要自行拋例外錯誤時，請直接使用   public static Exception CustomException(string errorkey) 這組函數功能。
        /// </summary>
        /// <param name="errorKey">當沒有例外狀況的資料內容時，可以透過此功能來直接紀錄錯誤資訊。有可能當下不是一個錯誤!所以可以透過此功能來做紀錄的動作。</param>
        /// <param name="skipLanguageTrans">是否要做語系的轉換? 如果是的話就需要寫false。</param>
        public static void Record(string errorKey, bool skipLanguageTrans)
        {
            try
            {
                lock (lockReport)
                {
                    if (errorKey.Length == 0 || errorKey.IndexOf(EXCEPTION_RECORD_CYMBOL, 0, 2) < 0)
                    {
                        // 轉換成系統設定的語系文字。
                        if (!skipLanguageTrans) errorKey = GetErrorMessage(errorKey);

                        // 填入時間資料與錯誤碼相對應的文字。再寫入暫存區內。
                        string strAllMessage = DateTime.Now.ToString(TIME_FORMAT) + ", " + errorKey;
                        LogMessage.Insert(0, strAllMessage);

                        // Log message count keep down 500 piece.
                        while (LogMessage.Count > recorderLines) LogMessage.RemoveAt(recorderLines);

                        // Trigger the event Recorded.
                        if (Recorded != null) Recorded(LogMessage[0]);
                    }
                }
            }
            catch
            {   // 直接忽略錯誤，因此區已經是錯誤處理區的最底層。
            }
        }

        /// <summary>傳入要紀錄的byte array，並且提供一個欲補充資料的字串來作紀錄。</summary>
        /// <param name="message">字串資料</param>
        /// <param name="byteArray"></param>
        public static void Record(string message, byte[] byteArray)
        {
            try
            {
                if (message.Length > 0)
                    Record(message + " : " + BitConverter.ToString(byteArray));
                else
                    Record(BitConverter.ToString(byteArray));
            }
            catch
            {   // 直接忽略錯誤，因此區已經是錯誤處理區的最底層。
            }
        }

        /// <summary>傳入要紀錄字串。</summary>
        /// <param name="message">字串資料</param>
        public static void Record(string message)
        {
            try
            {
                lock (lockReport)
                {
                    // 填入時間資料與錯誤碼相對應的文字。再寫入暫存區內。
                    string strAllMessage = DateTime.Now.ToString(TIME_FORMAT) + ", " + message;
                    LogMessage.Insert(0, strAllMessage);

                    // Log message count keep down 500 piece.
                    while (LogMessage.Count > 500) LogMessage.RemoveAt(500);

                    // Trigger the event Recorded.
                    if (Recorded != null) Recorded(LogMessage[0]);
                }
            }
            catch
            {   // 直接忽略錯誤，因此區已經是錯誤處理區的最底層。
            }
        }

        /// <summary>傳入要紀錄字串。</summary>
        /// <param name="message">字串資料</param>
        public static void Record(string[] message)
        {
            try
            {
                lock (lockReport)
                {
                    // 填入時間資料與錯誤碼相對應的文字。再寫入暫存區內。
                    string strAllMessage = DateTime.Now.ToString(TIME_FORMAT) + ", " + String.Join(Environment.NewLine, message);//+ "\t\t\t"
                    LogMessage.Insert(0, strAllMessage);

                    // Log message count keep down 500 piece.
                    while (LogMessage.Count > 500) LogMessage.RemoveAt(500);

                    // Trigger the event Recorded.
                    if (Recorded != null) Recorded(LogMessage[0]);
                }
            }
            catch
            {   // 直接忽略錯誤，因此區已經是錯誤處理區的最底層。
            }
        }


        /// <summary> 取得一個Exception，內部的訊息為自定義的錯誤訊息。透過此函數來作多國語系的錯誤碼管理。
        /// <para></para>建議填入ErrorCode.ER_SYSTEM 等常數資料。
        /// </summary>
        /// <param name="errorkey">請填入ErrorCode.ER_SYSTEM 等常數資料，請勿自行填寫字串。</param>
        /// <returns>回傳一個Exception，內部包含錯誤資料的語系轉換文字。</returns>
        public static Exception CustomException(string errorkey)
        {
            return new Exception(GetErrorMessage(errorkey));
        }

        /// <summary>透過Error key來回傳一個字串來形容此錯誤的真正訊息。在此會針對語系來回傳相對應的文字。
        /// <para></para>建議填入ErrorCode.ER_SYSTEM 等常數資料。
        /// </summary>
        /// <param name="errorKey">錯誤碼資料。</param>
        /// <returns>根據目前的語系來回傳一個對應的字串，用以形容該錯誤的一段文字。</returns>
        private static string GetErrorMessage(string errorKey)
        {
            try
            {
                return language.GetString(errorKey);
            }
            catch
            {
                // 直接回傳一段無法辨識的文字，不要發生錯誤traces在此，Debug的重點應該是別的行號。所以此區不拋出Exception。
                return "Error code was not defined. (" + errorKey + ")";
            }
        }

        #endregion

        #region " Events "

        /// <summary>當 有資訊回報時，要進行的事件觸發項目。</summary>
        public static event EventHandler Recorded;

        #endregion

    }
}