using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;

namespace Delta.Globalization
{
    /// <summary>處理語言相關的問題，包含多語系的資料庫取值。
    /// <para></para>目前這些工具是以Resource file的檔案資訊來做處理，不同的檔案語系會在檔案名稱之後有語系識別字。
    /// <para></para>譬如:  ResourcesName.es-ES	西班牙文 (西班牙)
    /// <para></para>       ResourcesName.zh-TW	中文 (台灣)
    /// <para></para>       ResourcesName.zh-CN	中文 (中華人民共和國)
    /// <para></para>       ResourcesName.zh-HK	中文 (香港特別行政區)
    /// <para></para>       ResourcesName.zh-SG	中文 (新加坡)
    /// <para></para>       ResourcesName.zh-MO	中文 (澳門特別行政區)
    /// <para></para>       ResourcesName.ja-JP	日文 (日本)
    /// <para></para>       ResourcesName.ko-KR	韓文 (韓國)
    /// <para></para>       ResourcesName.de-DE	德文 (德國)
    /// <para></para>       ResourcesName.hu-HU	匈牙利文 (匈牙利)
    /// <para></para>       ResourcesName.ka-GE	喬治亞文 (喬治亞)
    /// </summary>
    public class Language
    {

        #region " Definition "

        /// <summary>語系類型。</summary>
        public enum LanguageType
        {
            /// <summary>系統預設語系。</summary>
            None = 0,
            /// <summary>繁體中文。</summary>
            [Description("zh-tw")]
            zh_tw,
            /// <summary>英文。</summary>
            [Description("en-us")]
            en_us,
        }

        #endregion

        #region " Field "

        private string resourcesName = "ASD.General.Properties.Error";
        private LanguageType format = LanguageType.None;        
        private ResourceManager resourceManager;
        private CultureInfo cultureInfo;

        #endregion

        #region " Properties "

        /// <summary>讀取或設定語系的格式值。</summary>
        public LanguageType Format { get { return format; } set { SetFormat(value); } }

        #endregion

        #region " Methods - New "

        /// <summary> 建構一個多國語言的轉接單元，建立時需要提供目前使用的語系資源名稱。</summary>
        /// <param name="resourcesName">語言資源檔案名稱。如果是錯誤碼的多國語系資源檔，則可以輸入Error.zh_tw.resx。</param>
        public Language(string resourcesName)
        {
            if (resourcesName.Length > 0) this.resourcesName = resourcesName;
            resourceManager = new ResourceManager(this.resourcesName, Assembly.GetExecutingAssembly());
        }

        #endregion

        #region " Methods - Get/Set "

        /// <summary>設定語系的格式，讓使用者可以自訂目前的語系要輸出那種類型的語言文字。</summary>
        /// <param name="format">語系類型，如果設定預設的話，就討用目前OS的語系。</param>
        public void SetFormat(LanguageType format)
        {
            if (format == LanguageType.None)
            {
                cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;                
            }
            else
            {
                cultureInfo = new CultureInfo(format.GetDescription());
            }
        }

        /// <summary>根據目前語系的設定狀況來取得錯誤訊息的字串文字。</summary>
        /// <param name="key">定義於資源檔的文字資料。</param>
        /// <returns>回傳資源檔相對於key的文字。(根據目前類別所設定的語系去取得相對應的值)</returns>
        public string GetString(string key)
        {
            // 取得目前的語系文化設定值。
            if (this.Format == LanguageType.None)
                cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            else
                cultureInfo = new CultureInfo(this.Format.GetDescription());

            // 真正拿 Resource file 裡面資料的API工具函數是底下這行，取得後直接回拋。
            string message = this.resourceManager.GetString(key, cultureInfo);
            return message;
        }

        #endregion

    }
}

/// <summary>格外新增的功能管理類別。</summary>
internal static class ExtensionMethod
{
    /// <summary>取得該列舉的敘述文字。</summary>
    /// <param name="value">提供列舉的資料。</param>
    /// <returns>回傳列舉的文字內容。</returns>
    public static string GetDescription(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        var result = attributes.Length > 0 ? attributes[0].Description : value.ToString();
        return result;
    }
}