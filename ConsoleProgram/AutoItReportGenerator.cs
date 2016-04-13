using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;


namespace ConsoleProgram
{
    class AutoItReportGenerator : IConsoleProgram
    {
        string AutoItDirectory { get; set; }
        string ConfigFilePath { get; set; }
        string ExcelFileDirectory { get; set; }

        Excel.Application Excel { get; set; }
        Excel.Workbook Wb { get; set; }
        Excel.Worksheet Ws { get; set; }

        int TEST_CASE_START_POSITION = 14;

        public AutoItReportGenerator()
        {
            Excel = new Excel.Application();
            Excel.Visible = false;
            Excel.UserControl = true;  //宣告Excel唯讀

            string startup = System.AppDomain.CurrentDomain.BaseDirectory;
            string path = startup + @"ECATBuliderTestCaseReport.xlsx";
            Wb = Excel.Workbooks.Open(path, ReadOnly: true, Editable: true);
            Ws = Wb.Worksheets[1];
        }

        public bool Run()
        {
            bool result = InitPara();
            if (!result)
                return false;

            CreateDefaultCells();
            RunTestCase();

            result = SaveReport();
            if (!result)
                return false;

            Wb.Close(false, Type.Missing, Type.Missing);
            Excel.Quit();
            Console.WriteLine("Done");
            return true;
        }

        private bool InitPara()
        {
            Console.WriteLine("Initial parameters...");

            if (Program.ParaList.Count < 3)
                return false;

            AutoItDirectory = Program.ParaList[0];
            ConfigFilePath = Program.ParaList[1];
            ExcelFileDirectory = Program.ParaList[2];

            if (!Directory.Exists(AutoItDirectory) || !System.IO.File.Exists(ConfigFilePath))
                return false;

            if (!Directory.Exists(ExcelFileDirectory))
                Directory.CreateDirectory(ExcelFileDirectory);

            return true;
        }

        private void CreateDefaultCells()
        {
            Console.WriteLine("Create default value...");

            Ws.Cells[2, "B"] = "1";
            Ws.Cells[2, "D"] = "ECAT Builder";
            Ws.Cells[2, "F"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            Ws.Cells[3, "B"] = "何南瑾";
            Ws.Cells[3, "D"] = "3262";
            Ws.Cells[3, "F"] = "franknc.ho@deltaww.com";

            Ws.Cells[4, "D"] = "PLCBU";
            Ws.Cells[4, "F"] = "1";

            Configuration config = Configuration.Instance(ConfigFilePath);
            string[] autoItFiles = Directory.GetFiles(AutoItDirectory, "*.au3");

            for (int i = 0; i < autoItFiles.Length; i++)
            {
                string autoItName = System.IO.Path.GetFileNameWithoutExtension(autoItFiles[i]);
                int currentRow = TEST_CASE_START_POSITION + i;
                Ws.Cells[currentRow, "A"] = autoItName;
                Ws.Cells[currentRow, "B"] = "-";
                if (config.ConfigMap.ContainsKey(autoItName))
                    Ws.Cells[currentRow, "B"] = config.ConfigMap[autoItName].Description;

                Ws.Cells[currentRow, "D"] = "No";
                Ws.Cells[currentRow, "E"] = "Fail";
                Ws.Cells[currentRow, "F"] = "-";
            }

            Excel.Range wRange = Ws.Range[Ws.Cells[TEST_CASE_START_POSITION, "D"], Ws.Cells[TEST_CASE_START_POSITION + autoItFiles.Length, "E"]];
            wRange.Font.Color = ColorTranslator.ToOle(Color.Red);
        }

        private void RunTestCase()
        {
            Configuration config = Configuration.Instance(ConfigFilePath);
            string[] autoItFiles = Directory.GetFiles(AutoItDirectory, "*.au3");

            for (int i = 0; i < autoItFiles.Length; i++)
            {
                Console.WriteLine("Run test case " + (i + 1).ToString() + " / " + autoItFiles.Length.ToString() + "...");
                string autoItName = System.IO.Path.GetFileNameWithoutExtension(autoItFiles[i]);
                int timeout = 10 * 1000; // Default 60sec
                List<string> killProcessList = null;
                if (config.ConfigMap.ContainsKey(autoItName))
                {
                    timeout = config.ConfigMap[autoItName].Timeout * 1000;
                    killProcessList = config.ConfigMap[autoItName].KillProcessList;
                }

                string autoItResultFile = autoItFiles[i].Replace(".au3", ".txt");
                System.IO.File.Delete(autoItResultFile); // Delete txt file before running
   
                Process testProcess = new Process();
                testProcess.StartInfo.FileName = autoItFiles[i];
                testProcess.Start();

                Stopwatch sw = Stopwatch.StartNew();

                if (!testProcess.WaitForExit(timeout))
                    testProcess.Kill();

                sw.Stop();

                if (killProcessList != null) // Kill process
                {
                    foreach (string processName in killProcessList)
                        KillProcess(processName);
                }

                RecordTestCaseResult(i, testProcess.StartTime, sw.Elapsed, autoItResultFile);

                testProcess.Close();
                testProcess.Dispose();
            }
        }

        private void RecordTestCaseResult(int count, DateTime startTime, TimeSpan elapsedTime, string autoItResultFile)
        {
            int currentRow = TEST_CASE_START_POSITION + count;
            Ws.Cells[currentRow, "F"] = startTime.ToString("yyyy/mm/dd hh:mm:ss") + "\n" + elapsedTime.ToString("c");

            if (File.Exists(autoItResultFile))
            {
                Ws.Cells[currentRow, "D"] = "Yes";
                Excel.Range wRange = Ws.Range[Ws.Cells[currentRow, "D"], Ws.Cells[currentRow, "D"]];
                wRange.Font.Color = ColorTranslator.ToOle(Color.Green);

                using (StreamReader file = new System.IO.StreamReader(autoItResultFile))
                {
                    string runResult = file.ReadLine();
                    if (runResult.ToLower().Contains("pass"))
                    {
                        Ws.Cells[currentRow, "E"] = "Pass";
                        wRange = Ws.Range[Ws.Cells[currentRow, "E"], Ws.Cells[currentRow, "E"]];
                        wRange.Font.Color = ColorTranslator.ToOle(Color.Green);
                    }

                    file.Close();
                }

                File.Delete(autoItResultFile);
            }
        }

        private void KillProcess(string processName)
        {
            Process[] processAry = Process.GetProcessesByName(processName);
            foreach (Process p in processAry)
            {
                p.Kill();
            }
        }

        private bool SaveReport()
        {
            Console.WriteLine("Save Report...");
            try
            {
                string savePathFile = ExcelFileDirectory + @"/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
                Wb.SaveAs(savePathFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("儲存檔案出錯，檔案可能正在使用" + Environment.NewLine + ex.Message);
                return false;
            }

            return true;
        }
    }
}
