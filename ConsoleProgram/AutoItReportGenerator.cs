﻿using System;
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
        }

        public int Run()
        {
            bool result = InitPara();
            if (!result)
                return -1;

            CreateDefaultCells();
            RunTestCase();

            result = SaveReport();
            if (!result)
                return -2;

            return 0;
        }

        private bool InitPara()
        {
            Console.WriteLine("Initial...");

            if (Program.ParaList.Count < 3)
                return false;

            AutoItDirectory = Program.ParaList[0];
            ConfigFilePath = Program.ParaList[1];
            ExcelFileDirectory = Program.ParaList[2];

            if (!Directory.Exists(AutoItDirectory) || !System.IO.File.Exists(ConfigFilePath))
                return false;

            if (!Directory.Exists(ExcelFileDirectory))
                Directory.CreateDirectory(ExcelFileDirectory);

            string startup = System.AppDomain.CurrentDomain.BaseDirectory;
            string path = startup + @"ECATBuliderTestCaseReport.xlsx";
            try
            {
                Wb = Excel.Workbooks.Open(path, ReadOnly: true, Editable: true);
                Ws = Wb.Worksheets[1];
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }

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
                bool allPass = IsAllPass();
                string resultStr = allPass ? "(Pass)" : "(Fail)";
                string savePathFile = ExcelFileDirectory + @"\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + resultStr + ".xlsx";
                Wb.SaveAs(savePathFile);

                Wb.Close(false, Type.Missing, Type.Missing);
                Excel.Quit();

                MoveReportToPreFolder(savePathFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("儲存檔案出錯，檔案可能正在使用" + Environment.NewLine + ex.Message);
                return false;
            }

            return true;
        }

        private bool MoveReportToPreFolder(string savePathFile)
        {
            string preDirectory = ExcelFileDirectory + @"\previous";
            if (!Directory.Exists(preDirectory))
                Directory.CreateDirectory(preDirectory);

            string[] reportFiles = Directory.GetFiles(ExcelFileDirectory, "*.xlsx");

            for (int i = 0; i < reportFiles.Length; i++)
            {
                if (reportFiles[i] == savePathFile)
                    continue;

                string reportName = System.IO.Path.GetFileNameWithoutExtension(reportFiles[i]);
                File.Move(reportFiles[i], preDirectory + @"\" + reportName + ".xlsx"); // move to previous folder
            }

            return true;
        }

        private bool IsAllPass()
        {
            string failCountStr = Ws.Cells[10, "F"].Text;

            int failCount;
            bool result = Int32.TryParse(failCountStr, out failCount);
            if (!result)
                return false;

            if (failCount != 0)
                return false;

            return true;
        }
    }
}
