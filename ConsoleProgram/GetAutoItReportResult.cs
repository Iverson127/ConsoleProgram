using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;

namespace ConsoleProgram
{
    class GetAutoItReportResult : IConsoleProgram
    {
        string ReportFile { get; set; }

        Excel.Application Excel { get; set; }
        Excel.Workbook Wb { get; set; }
        Excel.Worksheet Ws { get; set; }

        public GetAutoItReportResult()
        {
            Excel = new Excel.Application();
            Excel.Visible = false;
            Excel.UserControl = true;  //宣告Excel唯讀
        }

        public bool Run()
        {
            bool result = InitPara();
            if (!result)
                return false;

            string failCountStr = Ws.Cells[10, "F"].Text;

            int failCount;
            result = Int32.TryParse(failCountStr, out failCount);
            if (!result)
                return false;

            if (failCount != 0)
                return false;

            return true;
        }

        private bool InitPara()
        {
            Console.WriteLine("Initial...");

            if (Program.ParaList.Count < 1)
                return false;

            ReportFile = Program.ParaList[0];

            if (!System.IO.File.Exists(ReportFile))
                return false;

            try
            {
                Wb = Excel.Workbooks.Open(ReportFile, ReadOnly: true);
                Ws = Wb.Worksheets[1];
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }

            return true;
        }
    }
}
