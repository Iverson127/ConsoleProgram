using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Delta
{
    /// <summary>錯誤碼定義類別。</summary>
    public static partial class ErrorCode
    {
        //  System Error ---------------------------------------------------------
        /// <summary>系統錯誤。</summary>
        public const string ER_SYSTEM = "System";
        /// <summary>系統錯誤，初始錯誤。</summary>
        public const string ER_INITIAL = "InitialFault.";
        /// <summary>系統錯誤，找不到裝置。</summary>
        public const string ER_NO_DEVICE = "DeviceNoFound.";
        /// <summary>系統錯誤，使用者介面錯誤。</summary>
        public const string ER_FILE_NOT_EXIST = "FileIsNotExist";
        /// <summary>系統錯誤，資料轉換失敗。</summary>
        public const string ER_DATA_TRANSFER = "DataTransfer";
        /// <summary>系統錯誤，未知的資料導致錯誤發生。</summary>
        public const string ER_UNKNOW_DATA = "UnknowData";

        //  Procedure Error ------------------------------------------------------
        /// <summary>程序錯誤。</summary>
        public const string ER_PROCEDURE = "ProcedureError";
        /// <summary>程序錯誤，參數錯誤。</summary>
        public const string ER_PARAMETER = "ParameterError";
        /// <summary>程序錯誤，逾時。</summary>
        public const string ER_TIMEOUT = "ProcedureTimeout";

        // Communication ---------------------------------------------------------
        /// <summary>通訊錯誤，通訊狀態未連接。</summary>
        public const string ER_COMM_NOT_CONNECT = "CommunicationNoConnect";
        /// <summary>通訊錯誤，通訊逾時。</summary>
        public const string ER_COMM_TIMEOUT = "CommunicationTimeout";
        /// <summary>站號錯誤，不符合。</summary>
        public const string ER_STATION_NUM = "StationNumberError";
        /// <summary>錯誤檢查錯誤。</summary>
        public const string ER_CHECKSUM = "ChecksumError";

        // =======================================================================
        // Warning Definition
        // 不屬於嚴重錯誤類型的定義型態。
        // =======================================================================

        // Interface -------------------------------------------------------------
        /// <summary>警告，Interface無此方法。</summary>
        public const string WN_INTERFACE_NO_FUNCTION = "InterfaceNoFunction";

        // Initialization --------------------------------------------------------
        /// <summary>警告，版本不同。</summary>
        public const string WN_VERSION_DIFFERENT = "VersionDifferent";

        // General ---------------------------------------------------------------
        /// <summary>警告，逾時。</summary>
        public const string WN_TIMEOUT = "Timeout";

    }
}